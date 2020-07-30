import { ObjectType, humanize } from '../../framework';

const icon = Symbol('icon');
const displayName = Symbol('displayName');

declare module '../../framework/meta/ObjectType' {
  interface ObjectType {
    icon: string;
    displayName: string;
    list: string;
    overview: string;
  }
}

Object.defineProperty(ObjectType.prototype, 'icon', {
  get(this: ObjectType): string {
    return (this as any)[icon];
  },

  set(this: ObjectType, value: string): void {
    (this as any)[icon] = value;
  },
});

Object.defineProperty(ObjectType.prototype, 'displayName', {
  get(this: ObjectType): string {
    return (this as any)[displayName] || humanize(this.name);
  },

  set(this: ObjectType, value: string): void {
    (this as any)[displayName] = value;
  },
});
