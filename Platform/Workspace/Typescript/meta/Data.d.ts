export interface Data {
    domains: string[];
    interfaces?: Interface[];
    classes?: Class[];
}
export interface Interface {
    name: string;
    id: string;
    interfaces?: string[];
    exclusiveRoleTypes?: RoleType[];
    associationTypes?: AssociationType[];
    exclusiveMethodTypes?: MethodType[];
}
export interface Class {
    name: string;
    id: string;
    interfaces?: string[];
    exclusiveRoleTypes?: RoleType[];
    concreteRoleTypes?: ConcreteRoleType[];
    associationTypes?: AssociationType[];
    exclusiveMethodTypes?: MethodType[];
    concreteMethodTypes?: ConcreteMethodType[];
}
export interface RoleType {
    name: string;
    id: string;
    objectType: string;
    isUnit: boolean;
    isOne: boolean;
    isRequired: boolean;
}
export interface ConcreteRoleType {
    id: string;
    isRequired: boolean;
}
export interface AssociationType {
    name: string;
    id: string;
}
export interface MethodType {
    name: string;
    id: string;
}
export interface ConcreteMethodType {
    id: string;
}
