// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneCommunication.cs" company="Allors bvba">
//   Copyright 2002-2012 Allors bvba.
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// Allors Applications is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Allors.Meta;
using Resources;

namespace Allors.Domain
{
    public partial class PhoneCommunication
    {
        ObjectState Transitional.CurrentObjectState => this.CurrentObjectState;

        public void AppsOnDerive(ObjectOnDerive method)
        {
            this.AppsOnDeriveFromParties();
            this.AppsOnDeriveToParties();
            this.AppsOnDeriveInvolvedParties();
        }

        public void AppsOnPostDerive(ObjectOnPostDerive method)
        {
            var isNewVersion =
                !this.ExistCurrentVersion ||
                !object.Equals(this.ActualStart, this.CurrentVersion.ActualStart);

            var isNewStateVersion =
                !this.ExistCurrentVersion ||
                !object.Equals(this.ScheduledStart, this.CurrentVersion.ScheduledStart) ||
                !object.Equals(this.ContactMechanisms, this.CurrentVersion.ContactMechanisms) ||
                !object.Equals(this.InitialScheduledStart, this.CurrentVersion.InitialScheduledStart) ||
                !object.Equals(this.EventPurposes, this.CurrentVersion.EventPurposes) ||
                !object.Equals(this.ScheduledEnd, this.CurrentVersion.ScheduledEnd) ||
                !object.Equals(this.ActualEnd, this.CurrentVersion.ActualEnd) ||
                !object.Equals(this.WorkEfforts, this.CurrentVersion.WorkEfforts) ||
                !object.Equals(this.Description, this.CurrentVersion.Description) ||
                !object.Equals(this.InitialScheduledEnd, this.CurrentVersion.InitialScheduledEnd) ||
                !object.Equals(this.Subject, this.CurrentVersion.Subject) ||
                !object.Equals(this.Documents, this.CurrentVersion.Documents) ||
                !object.Equals(this.Case, this.CurrentVersion.Case) ||
                !object.Equals(this.Priority, this.CurrentVersion.Priority) ||
                !object.Equals(this.Owner, this.CurrentVersion.Owner) ||
                !object.Equals(this.Note, this.CurrentVersion.Note) ||
                !object.Equals(this.ActualStart, this.CurrentVersion.ActualStart) ||
                !object.Equals(this.SendNotification, this.CurrentVersion.SendNotification) ||
                !object.Equals(this.SendReminder, this.CurrentVersion.SendReminder) ||
                !object.Equals(this.RemindAt, this.CurrentVersion.RemindAt) ||
                !object.Equals(this.LeftVoiceMail, this.CurrentVersion.LeftVoiceMail) ||
                !object.Equals(this.IncomingCall, this.CurrentVersion.IncomingCall) ||
                !object.Equals(this.Receivers, this.CurrentVersion.Receivers) ||
                !object.Equals(this.Callers, this.CurrentVersion.Callers) ||
                !object.Equals(this.CurrentObjectState, this.CurrentVersion.CurrentObjectState);

            if (isNewVersion)
            {
                this.PreviousVersion = this.CurrentVersion;
                this.CurrentVersion = new PhoneCommunicationVersionBuilder(this.Strategy.Session).WithPhoneCommunication(this).Build();
                this.AddAllVersion(this.CurrentVersion);
            }

            if (isNewStateVersion)
            {
                this.CurrentStateVersion = CurrentVersion;
                this.AddAllStateVersion(this.CurrentStateVersion);
            }
        }

        public void AppsOnDeriveFromParties()
        {
            this.RemoveFromParties();
            this.FromParties = (Extent) this.Callers;

            if (this.IncomingCall)
            {
                foreach (Party party in this.PartyRelationshipWhereCommunicationEvent.Parties)
                {
                    this.AddFromParty(party);
                }
            }
        }

        public void AppsOnDeriveToParties()
        {
            this.RemoveToParties();
            this.ToParties = (Extent)this.Receivers;

            if (this.ExistPartyRelationshipWhereCommunicationEvent && !this.IncomingCall)
            {
                foreach (Party party in this.PartyRelationshipWhereCommunicationEvent.Parties)
                {
                    this.AddToParty(party);
                }
            }
        }

        public void AppsOnDeriveInvolvedParties()
        {
            this.RemoveInvolvedParties();

            foreach (Party party in this.FromParties)
            {
                this.AddInvolvedParty(party);
            }

            foreach (Party party in this.ToParties)
            {
                this.AddInvolvedParty(party);
            }

            //if (this.ExistOwner && !this.InvolvedParties.Contains(this.Owner))
            //{
            //    this.AddInvolvedParty(this.Owner);
            //}
        }
    }
}
