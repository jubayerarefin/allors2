// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { Version } from './Version.g';
import { CommunicationEventState } from './CommunicationEventState.g';
import { User } from './User.g';
import { Party } from './Party.g';
import { ContactMechanism } from './ContactMechanism.g';
import { CommunicationEventPurpose } from './CommunicationEventPurpose.g';
import { WorkEffort } from './WorkEffort.g';
import { Media } from './Media.g';
import { Case } from './Case.g';
import { Priority } from './Priority.g';
import { Person } from './Person.g';

export interface CommunicationEventVersion extends SessionObject , Version {
        CommunicationEventState : CommunicationEventState;


        Comment : string;


        CreatedBy : User;


        LastModifiedBy : User;


        CreationDate : Date;


        LastModifiedDate : Date;


        ScheduledStart : Date;


        ToParties : Party[];


        ContactMechanisms : ContactMechanism[];
        AddContactMechanism(value: ContactMechanism);
        RemoveContactMechanism(value: ContactMechanism);


        InvolvedParties : Party[];


        InitialScheduledStart : Date;


        EventPurposes : CommunicationEventPurpose[];
        AddEventPurpose(value: CommunicationEventPurpose);
        RemoveEventPurpose(value: CommunicationEventPurpose);


        ScheduledEnd : Date;


        ActualEnd : Date;


        WorkEfforts : WorkEffort[];
        AddWorkEffort(value: WorkEffort);
        RemoveWorkEffort(value: WorkEffort);


        Description : string;


        InitialScheduledEnd : Date;


        FromParties : Party[];


        Subject : string;


        Documents : Media[];
        AddDocument(value: Media);
        RemoveDocument(value: Media);


        Case : Case;


        Priority : Priority;


        Owner : Person;


        Note : string;


        ActualStart : Date;


        SendNotification : boolean;


        SendReminder : boolean;


        RemindAt : Date;


}