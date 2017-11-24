// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { PartyRelationship } from './PartyRelationship.g';
import { Period } from './Period.g';
import { Deletable } from './Deletable.g';
import { Person } from './Person.g';
import { Organisation } from './Organisation.g';
import { Party } from './Party.g';

export class ProfessionalServicesRelationship extends SessionObject implements PartyRelationship {
    get CanReadProfessional(): boolean {
        return this.canRead('Professional');
    }

    get CanWriteProfessional(): boolean {
        return this.canWrite('Professional');
    }

    get Professional(): Person {
        return this.get('Professional');
    }

    set Professional(value: Person) {
        this.set('Professional', value);
    }

    get CanReadProfessionalServicesProvider(): boolean {
        return this.canRead('ProfessionalServicesProvider');
    }

    get CanWriteProfessionalServicesProvider(): boolean {
        return this.canWrite('ProfessionalServicesProvider');
    }

    get ProfessionalServicesProvider(): Organisation {
        return this.get('ProfessionalServicesProvider');
    }

    set ProfessionalServicesProvider(value: Organisation) {
        this.set('ProfessionalServicesProvider', value);
    }

    get CanReadParties(): boolean {
        return this.canRead('Parties');
    }

    get Parties(): Party[] {
        return this.get('Parties');
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
