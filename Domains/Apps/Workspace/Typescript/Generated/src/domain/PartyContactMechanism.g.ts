// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Commentable } from './Commentable.g';
import { Auditable } from './Auditable.g';
import { Period } from './Period.g';
import { Deletable } from './Deletable.g';
import { ContactMechanismPurpose } from './ContactMechanismPurpose.g';
import { ContactMechanism } from './ContactMechanism.g';
import { User } from './User.g';
import { PartyVersion } from './PartyVersion.g';
import { Party } from './Party.g';

export class PartyContactMechanism extends SessionObject implements Commentable, Auditable, Period, Deletable {
    get CanReadContactPurposes(): boolean {
        return this.canRead('ContactPurposes');
    }

    get CanWriteContactPurposes(): boolean {
        return this.canWrite('ContactPurposes');
    }

    get ContactPurposes(): ContactMechanismPurpose[] {
        return this.get('ContactPurposes');
    }

    AddContactPurpose(value: ContactMechanismPurpose) {
        return this.add('ContactPurposes', value);
    }

    RemoveContactPurpose(value: ContactMechanismPurpose) {
        return this.remove('ContactPurposes', value);
    }

    set ContactPurposes(value: ContactMechanismPurpose[]) {
        this.set('ContactPurposes', value);
    }

    get CanReadContactMechanism(): boolean {
        return this.canRead('ContactMechanism');
    }

    get CanWriteContactMechanism(): boolean {
        return this.canWrite('ContactMechanism');
    }

    get ContactMechanism(): ContactMechanism {
        return this.get('ContactMechanism');
    }

    set ContactMechanism(value: ContactMechanism) {
        this.set('ContactMechanism', value);
    }

    get CanReadUseAsDefault(): boolean {
        return this.canRead('UseAsDefault');
    }

    get CanWriteUseAsDefault(): boolean {
        return this.canWrite('UseAsDefault');
    }

    get UseAsDefault(): boolean {
        return this.get('UseAsDefault');
    }

    set UseAsDefault(value: boolean) {
        this.set('UseAsDefault', value);
    }

    get CanReadNonSolicitationIndicator(): boolean {
        return this.canRead('NonSolicitationIndicator');
    }

    get CanWriteNonSolicitationIndicator(): boolean {
        return this.canWrite('NonSolicitationIndicator');
    }

    get NonSolicitationIndicator(): boolean {
        return this.get('NonSolicitationIndicator');
    }

    set NonSolicitationIndicator(value: boolean) {
        this.set('NonSolicitationIndicator', value);
    }

    get CanReadComment(): boolean {
        return this.canRead('Comment');
    }

    get CanWriteComment(): boolean {
        return this.canWrite('Comment');
    }

    get Comment(): string {
        return this.get('Comment');
    }

    set Comment(value: string) {
        this.set('Comment', value);
    }

    get CanReadCreatedBy(): boolean {
        return this.canRead('CreatedBy');
    }

    get CanWriteCreatedBy(): boolean {
        return this.canWrite('CreatedBy');
    }

    get CreatedBy(): User {
        return this.get('CreatedBy');
    }

    set CreatedBy(value: User) {
        this.set('CreatedBy', value);
    }

    get CanReadLastModifiedBy(): boolean {
        return this.canRead('LastModifiedBy');
    }

    get CanWriteLastModifiedBy(): boolean {
        return this.canWrite('LastModifiedBy');
    }

    get LastModifiedBy(): User {
        return this.get('LastModifiedBy');
    }

    set LastModifiedBy(value: User) {
        this.set('LastModifiedBy', value);
    }

    get CanReadCreationDate(): boolean {
        return this.canRead('CreationDate');
    }

    get CanWriteCreationDate(): boolean {
        return this.canWrite('CreationDate');
    }

    get CreationDate(): Date {
        return this.get('CreationDate');
    }

    set CreationDate(value: Date) {
        this.set('CreationDate', value);
    }

    get CanReadLastModifiedDate(): boolean {
        return this.canRead('LastModifiedDate');
    }

    get CanWriteLastModifiedDate(): boolean {
        return this.canWrite('LastModifiedDate');
    }

    get LastModifiedDate(): Date {
        return this.get('LastModifiedDate');
    }

    set LastModifiedDate(value: Date) {
        this.set('LastModifiedDate', value);
    }

    get CanReadFromDate(): boolean {
        return this.canRead('FromDate');
    }

    get CanWriteFromDate(): boolean {
        return this.canWrite('FromDate');
    }

    get FromDate(): Date {
        return this.get('FromDate');
    }

    set FromDate(value: Date) {
        this.set('FromDate', value);
    }

    get CanReadThroughDate(): boolean {
        return this.canRead('ThroughDate');
    }

    get CanWriteThroughDate(): boolean {
        return this.canWrite('ThroughDate');
    }

    get ThroughDate(): Date {
        return this.get('ThroughDate');
    }

    set ThroughDate(value: Date) {
        this.set('ThroughDate', value);
    }


    get CanExecuteDelete(): boolean {
        return this.canExecute('Delete');
    }

    get Delete(): Method {
        return new Method(this, 'Delete');
    }
}
