// <copyright file="Security.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;

    using Allors;
    using Allors.Meta;

    public partial class Security
    {
        private static readonly Operations[] ReadWriteExecute = { Operations.Read, Operations.Write, Operations.Execute };

        private readonly ISession session;
        private readonly Dictionary<ObjectType, IObjects> objectsByObjectType;

        private readonly Dictionary<Guid, Role> roleById;
        private readonly Dictionary<Guid, Dictionary<OperandType, Permission>> readPermissionsByObjectTypeId;
        private readonly Dictionary<Guid, Dictionary<OperandType, Permission>> writePermissionsByObjectTypeId;
        private readonly Dictionary<Guid, Dictionary<OperandType, Permission>> executePermissionsByObjectTypeId;

        private readonly Dictionary<Guid, Dictionary<Guid, Permission>> deniablePermissionByOperandTypeIdByObjectTypeId;

        public Security(ISession session)
        {
            this.session = session;

            this.objectsByObjectType = new Dictionary<ObjectType, IObjects>();
            foreach (ObjectType objectType in session.Database.MetaPopulation.Composites)
            {
                this.objectsByObjectType[objectType] = objectType.GetObjects(session);
            }

            this.roleById = new Dictionary<Guid, Role>();
            foreach (Role role in session.Extent<Role>())
            {
                if (!role.ExistUniqueId)
                {
                    throw new Exception("Role " + role + " has no unique id");
                }

                this.roleById[role.UniqueId] = role;
            }

            this.readPermissionsByObjectTypeId = new Dictionary<Guid, Dictionary<OperandType, Permission>>();
            this.writePermissionsByObjectTypeId = new Dictionary<Guid, Dictionary<OperandType, Permission>>();
            this.executePermissionsByObjectTypeId = new Dictionary<Guid, Dictionary<OperandType, Permission>>();

            this.deniablePermissionByOperandTypeIdByObjectTypeId = new Dictionary<Guid, Dictionary<Guid, Permission>>();

            foreach (Permission permission in session.Extent<Permission>())
            {
                if (!permission.ExistConcreteClassPointer || !permission.ExistOperandTypePointer || !permission.ExistOperation)
                {
                    throw new Exception("Permission " + permission + " has no concrete class, operand type and/or operation");
                }

                var objectId = permission.ConcreteClassPointer;

                if (permission.Operation != Operations.Read)
                {
                    var operandType = permission.OperandTypePointer;

                    if (!this.deniablePermissionByOperandTypeIdByObjectTypeId.TryGetValue(objectId, out var deniablePermissionByOperandTypeId))
                    {
                        deniablePermissionByOperandTypeId = new Dictionary<Guid, Permission>();
                        this.deniablePermissionByOperandTypeIdByObjectTypeId[objectId] = deniablePermissionByOperandTypeId;
                    }

                    deniablePermissionByOperandTypeId.Add(operandType, permission);
                }

                Dictionary<Guid, Dictionary<OperandType, Permission>> permissionByOperandTypeByObjectTypeId;
                switch (permission.Operation)
                {
                    case Operations.Read:
                        permissionByOperandTypeByObjectTypeId = this.readPermissionsByObjectTypeId;
                        break;

                    case Operations.Write:
                        permissionByOperandTypeByObjectTypeId = this.writePermissionsByObjectTypeId;
                        break;

                    case Operations.Execute:
                        permissionByOperandTypeByObjectTypeId = this.executePermissionsByObjectTypeId;
                        break;

                    default:
                        throw new Exception("Unkown operation: " + permission.Operation);
                }

                if (!permissionByOperandTypeByObjectTypeId.TryGetValue(objectId, out var permissionByOperandType))
                {
                    permissionByOperandType = new Dictionary<OperandType, Permission>();
                    permissionByOperandTypeByObjectTypeId[objectId] = permissionByOperandType;
                }

                if (permission.OperandType == null)
                {
                    permission.Delete();
                }
                else
                {
                    permissionByOperandType.Add(permission.OperandType, permission);
                }
            }
        }

        public void Apply()
        {
            foreach (Role role in this.session.Extent<Role>())
            {
                role.RemovePermissions();
                role.RemoveDeniedPermissions();
            }

            this.OnPreSetup();

            foreach (var objects in this.objectsByObjectType.Values)
            {
                objects.Secure(this);
            }

            this.OnPostSetup();

            this.session.Derive();
        }

        public void Deny(ObjectType objectType, ObjectState objectState, params Operations[] operations)
        {
            var actualOperations = operations ?? ReadWriteExecute;
            foreach (var operation in actualOperations)
            {
                Dictionary<OperandType, Permission> permissionByOperandType;
                switch (operation)
                {
                    case Operations.Read:
                        this.readPermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                        break;

                    case Operations.Write:
                        this.writePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                        break;

                    case Operations.Execute:
                        this.executePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                        break;

                    default:
                        throw new Exception("Unkown operation: " + operations);
                }

                if (permissionByOperandType != null)
                {
                    foreach (var dictionaryEntry in permissionByOperandType)
                    {
                        objectState.AddDeniedPermission(dictionaryEntry.Value);
                    }
                }
            }
        }

        public void Deny(ObjectType objectType, ObjectState objectState, params OperandType[] operandTypes) => this.Deny(objectType, objectState, (IEnumerable<OperandType>)operandTypes);

        public void Deny(ObjectType objectType, ObjectState objectState, IEnumerable<OperandType> operandTypes)
        {
            if (this.deniablePermissionByOperandTypeIdByObjectTypeId.TryGetValue(objectType.Id, out var deniablePermissionByOperandTypeId))
            {
                foreach (var operandType in operandTypes)
                {
                    if (deniablePermissionByOperandTypeId.TryGetValue(operandType.Id, out var permission))
                    {
                        objectState.AddDeniedPermission(permission);
                    }
                }
            }
        }

        public void Grant(Guid roleId, ObjectType objectType, params Operations[] operations)
        {
            if (this.roleById.TryGetValue(roleId, out var role))
            {
                var actualOperations = operations ?? ReadWriteExecute;
                foreach (var operation in actualOperations)
                {
                    Dictionary<OperandType, Permission> permissionByOperandType;
                    switch (operation)
                    {
                        case Operations.Read:
                            this.readPermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        case Operations.Write:
                            this.writePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        case Operations.Execute:
                            this.executePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        default:
                            throw new Exception("Unkown operation: " + operations);
                    }

                    if (permissionByOperandType != null)
                    {
                        foreach (var dictionaryEntry in permissionByOperandType)
                        {
                            role.AddPermission(dictionaryEntry.Value);
                        }
                    }
                }
            }
        }

        public void GrantExcept(Guid roleId, ObjectType objectType, ICollection<OperandType> excepts, params Operations[] operations)
        {
            if (this.roleById.TryGetValue(roleId, out var role))
            {
                var actualOperations = operations ?? ReadWriteExecute;
                foreach (var operation in actualOperations)
                {
                    Dictionary<OperandType, Permission> permissionByOperandType;
                    switch (operation)
                    {
                        case Operations.Read:
                            this.readPermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        case Operations.Write:
                            this.writePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        case Operations.Execute:
                            this.executePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        default:
                            throw new Exception("Unkown operation: " + operations);
                    }

                    if (permissionByOperandType != null)
                    {
                        foreach (var dictionaryEntry in permissionByOperandType.Where(v => !excepts.Contains(v.Key)))
                        {
                            role.AddPermission(dictionaryEntry.Value);
                        }
                    }
                }
            }
        }

        public void Grant(Guid roleId, ObjectType objectType, OperandType operandType, params Operations[] operations)
        {
            if (this.roleById.TryGetValue(roleId, out var role))
            {
                var actualOperations = operations ?? ReadWriteExecute;
                foreach (var operation in actualOperations)
                {
                    Dictionary<OperandType, Permission> permissionByOperandType;
                    switch (operation)
                    {
                        case Operations.Read:
                            this.readPermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        case Operations.Write:
                            this.writePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        case Operations.Execute:
                            this.executePermissionsByObjectTypeId.TryGetValue(objectType.Id, out permissionByOperandType);
                            break;

                        default:
                            throw new Exception("Unkown operation: " + operations);
                    }

                    if (permissionByOperandType != null)
                    {
                        if (permissionByOperandType.TryGetValue(operandType, out var permission))
                        {
                            role.AddPermission(permission);
                        }
                    }
                }
            }
        }

        public void GrantAdministrator(ObjectType objectType, params Operations[] operations) => this.Grant(Roles.AdministratorId, objectType, operations);

        public void GrantGuest(ObjectType objectType, params Operations[] operations) => this.Grant(Roles.GuestId, objectType, operations);

        public void GrantCreator(ObjectType objectType, params Operations[] operations) => this.Grant(Roles.CreatorId, objectType, operations);

        public void GrantOwner(ObjectType objectType, params Operations[] operations) => this.Grant(Roles.OwnerId, objectType, operations);

        private void CoreOnPreSetup()
        {
        }

        private void CoreOnPostSetup()
        {
        }
    }
}
