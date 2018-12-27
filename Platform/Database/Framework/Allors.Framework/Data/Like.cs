//------------------------------------------------------------------------------------------------- 
// <copyright file="Like.cs" company="Allors bvba">
// Copyright 2002-2017 Allors bvba.
// 
// Dual Licensed under
//   a) the Lesser General Public Licence v3 (LGPL)
//   b) the Allors License
// 
// The LGPL License is included in the file lgpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Allors.Data
{
    using System.Collections.Generic;

    using Allors.Data.Protocol;
    using Allors.Meta;

    public class Like : IRolePredicate
    {
        public Like(IRoleType roleType = null)
        {
            this.RoleType = roleType;
        }

        public IRoleType RoleType { get; set; }

        public string Value { get; set; }

        public string Parameter { get; set; }

        public Predicate Save()
        {
            return new Predicate
                       {
                           Kind = PredicateKind.Like,
                           RoleType = this.RoleType?.Id,
                           Value = Convert.ToString(this.Value),
                           Parameter = this.Parameter
                       };
        }

        bool IPredicate.ShouldTreeShake(IReadOnlyDictionary<string, object> arguments)
        {
            return ((IPredicate)this).HasMissingArguments(arguments);
        }

        bool IPredicate.HasMissingArguments(IReadOnlyDictionary<string, object> arguments)
        {
            return this.Parameter != null && arguments != null && !arguments.ContainsKey(this.Parameter);
        }

        void IPredicate.Build(ISession session, IReadOnlyDictionary<string, object> arguments, Allors.ICompositePredicate compositePredicate)
        {
            object argument = null;
            if (this.Parameter != null)
            {
                if (arguments == null || !arguments.TryGetValue(this.Parameter, out argument))
                {
                    return;
                }
            }

            var value = this.Parameter != null ? (string)argument : this.Value;

            compositePredicate.AddLike(this.RoleType, value);
        }
    }
}