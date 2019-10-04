import { Permission } from './Permission';

export class AccessControl {
  constructor(public id: string, public version: string, public permissions: Set<Permission>) {
  }
}
