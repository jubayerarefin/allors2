// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { PartyVersion } from './PartyVersion.g';
import { Version } from './Version.g';
import { User } from './User.g';
import { PostalAddress } from './PostalAddress.g';
import { TelecommunicationsNumber } from './TelecommunicationsNumber.g';
import { Qualification } from './Qualification.g';
import { ContactMechanism } from './ContactMechanism.g';
import { OrganisationContactRelationship } from './OrganisationContactRelationship.g';
import { Person } from './Person.g';
import { PartyContactMechanism } from './PartyContactMechanism.g';
import { BillingAccount } from './BillingAccount.g';
import { PartySkill } from './PartySkill.g';
import { PartyClassification } from './PartyClassification.g';
import { BankAccount } from './BankAccount.g';
import { ElectronicAddress } from './ElectronicAddress.g';
import { ShipmentMethod } from './ShipmentMethod.g';
import { Resume } from './Resume.g';
import { Media } from './Media.g';
import { CreditCard } from './CreditCard.g';
import { PaymentMethod } from './PaymentMethod.g';
import { Currency } from './Currency.g';
import { VatRegime } from './VatRegime.g';
import { DunningType } from './DunningType.g';
import { AutomatedAgent } from './AutomatedAgent.g';

export class AutomatedAgentVersion extends SessionObject implements PartyVersion {
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

    get CanReadPartyName(): boolean {
        return this.canRead('PartyName');
    }

    get CanWritePartyName(): boolean {
        return this.canWrite('PartyName');
    }

    get PartyName(): string {
        return this.get('PartyName');
    }

    set PartyName(value: string) {
        this.set('PartyName', value);
    }

    get CanReadGeneralCorrespondence(): boolean {
        return this.canRead('GeneralCorrespondence');
    }

    get GeneralCorrespondence(): PostalAddress {
        return this.get('GeneralCorrespondence');
    }


    get CanReadYTDRevenue(): boolean {
        return this.canRead('YTDRevenue');
    }

    get YTDRevenue(): number {
        return this.get('YTDRevenue');
    }


    get CanReadLastYearsRevenue(): boolean {
        return this.canRead('LastYearsRevenue');
    }

    get LastYearsRevenue(): number {
        return this.get('LastYearsRevenue');
    }


    get CanReadBillingInquiriesFax(): boolean {
        return this.canRead('BillingInquiriesFax');
    }

    get BillingInquiriesFax(): TelecommunicationsNumber {
        return this.get('BillingInquiriesFax');
    }


    get CanReadQualifications(): boolean {
        return this.canRead('Qualifications');
    }

    get CanWriteQualifications(): boolean {
        return this.canWrite('Qualifications');
    }

    get Qualifications(): Qualification[] {
        return this.get('Qualifications');
    }

    AddQualification(value: Qualification) {
        return this.add('Qualifications', value);
    }

    RemoveQualification(value: Qualification) {
        return this.remove('Qualifications', value);
    }

    set Qualifications(value: Qualification[]) {
        this.set('Qualifications', value);
    }

    get CanReadHomeAddress(): boolean {
        return this.canRead('HomeAddress');
    }

    get HomeAddress(): ContactMechanism {
        return this.get('HomeAddress');
    }


    get CanReadInactiveOrganisationContactRelationships(): boolean {
        return this.canRead('InactiveOrganisationContactRelationships');
    }

    get InactiveOrganisationContactRelationships(): OrganisationContactRelationship[] {
        return this.get('InactiveOrganisationContactRelationships');
    }


    get CanReadSalesOffice(): boolean {
        return this.canRead('SalesOffice');
    }

    get SalesOffice(): ContactMechanism {
        return this.get('SalesOffice');
    }


    get CanReadInactiveContacts(): boolean {
        return this.canRead('InactiveContacts');
    }

    get InactiveContacts(): Person[] {
        return this.get('InactiveContacts');
    }


    get CanReadInactivePartyContactMechanisms(): boolean {
        return this.canRead('InactivePartyContactMechanisms');
    }

    get InactivePartyContactMechanisms(): PartyContactMechanism[] {
        return this.get('InactivePartyContactMechanisms');
    }


    get CanReadOrderInquiriesFax(): boolean {
        return this.canRead('OrderInquiriesFax');
    }

    get OrderInquiriesFax(): TelecommunicationsNumber {
        return this.get('OrderInquiriesFax');
    }


    get CanReadCurrentSalesReps(): boolean {
        return this.canRead('CurrentSalesReps');
    }

    get CurrentSalesReps(): Person[] {
        return this.get('CurrentSalesReps');
    }


    get CanReadPartyContactMechanisms(): boolean {
        return this.canRead('PartyContactMechanisms');
    }

    get CanWritePartyContactMechanisms(): boolean {
        return this.canWrite('PartyContactMechanisms');
    }

    get PartyContactMechanisms(): PartyContactMechanism[] {
        return this.get('PartyContactMechanisms');
    }

    AddPartyContactMechanism(value: PartyContactMechanism) {
        return this.add('PartyContactMechanisms', value);
    }

    RemovePartyContactMechanism(value: PartyContactMechanism) {
        return this.remove('PartyContactMechanisms', value);
    }

    set PartyContactMechanisms(value: PartyContactMechanism[]) {
        this.set('PartyContactMechanisms', value);
    }

    get CanReadShippingInquiriesFax(): boolean {
        return this.canRead('ShippingInquiriesFax');
    }

    get ShippingInquiriesFax(): TelecommunicationsNumber {
        return this.get('ShippingInquiriesFax');
    }


    get CanReadShippingInquiriesPhone(): boolean {
        return this.canRead('ShippingInquiriesPhone');
    }

    get ShippingInquiriesPhone(): TelecommunicationsNumber {
        return this.get('ShippingInquiriesPhone');
    }


    get CanReadBillingAccounts(): boolean {
        return this.canRead('BillingAccounts');
    }

    get CanWriteBillingAccounts(): boolean {
        return this.canWrite('BillingAccounts');
    }

    get BillingAccounts(): BillingAccount[] {
        return this.get('BillingAccounts');
    }

    AddBillingAccount(value: BillingAccount) {
        return this.add('BillingAccounts', value);
    }

    RemoveBillingAccount(value: BillingAccount) {
        return this.remove('BillingAccounts', value);
    }

    set BillingAccounts(value: BillingAccount[]) {
        this.set('BillingAccounts', value);
    }

    get CanReadOrderInquiriesPhone(): boolean {
        return this.canRead('OrderInquiriesPhone');
    }

    get OrderInquiriesPhone(): TelecommunicationsNumber {
        return this.get('OrderInquiriesPhone');
    }


    get CanReadPartySkills(): boolean {
        return this.canRead('PartySkills');
    }

    get CanWritePartySkills(): boolean {
        return this.canWrite('PartySkills');
    }

    get PartySkills(): PartySkill[] {
        return this.get('PartySkills');
    }

    AddPartySkill(value: PartySkill) {
        return this.add('PartySkills', value);
    }

    RemovePartySkill(value: PartySkill) {
        return this.remove('PartySkills', value);
    }

    set PartySkills(value: PartySkill[]) {
        this.set('PartySkills', value);
    }

    get CanReadPartyClassifications(): boolean {
        return this.canRead('PartyClassifications');
    }

    get PartyClassifications(): PartyClassification[] {
        return this.get('PartyClassifications');
    }


    get CanReadExcludeFromDunning(): boolean {
        return this.canRead('ExcludeFromDunning');
    }

    get CanWriteExcludeFromDunning(): boolean {
        return this.canWrite('ExcludeFromDunning');
    }

    get ExcludeFromDunning(): boolean {
        return this.get('ExcludeFromDunning');
    }

    set ExcludeFromDunning(value: boolean) {
        this.set('ExcludeFromDunning', value);
    }

    get CanReadBankAccounts(): boolean {
        return this.canRead('BankAccounts');
    }

    get CanWriteBankAccounts(): boolean {
        return this.canWrite('BankAccounts');
    }

    get BankAccounts(): BankAccount[] {
        return this.get('BankAccounts');
    }

    AddBankAccount(value: BankAccount) {
        return this.add('BankAccounts', value);
    }

    RemoveBankAccount(value: BankAccount) {
        return this.remove('BankAccounts', value);
    }

    set BankAccounts(value: BankAccount[]) {
        this.set('BankAccounts', value);
    }

    get CanReadCurrentContacts(): boolean {
        return this.canRead('CurrentContacts');
    }

    get CurrentContacts(): Person[] {
        return this.get('CurrentContacts');
    }


    get CanReadBillingAddress(): boolean {
        return this.canRead('BillingAddress');
    }

    get BillingAddress(): ContactMechanism {
        return this.get('BillingAddress');
    }


    get CanReadGeneralEmail(): boolean {
        return this.canRead('GeneralEmail');
    }

    get GeneralEmail(): ElectronicAddress {
        return this.get('GeneralEmail');
    }


    get CanReadDefaultShipmentMethod(): boolean {
        return this.canRead('DefaultShipmentMethod');
    }

    get CanWriteDefaultShipmentMethod(): boolean {
        return this.canWrite('DefaultShipmentMethod');
    }

    get DefaultShipmentMethod(): ShipmentMethod {
        return this.get('DefaultShipmentMethod');
    }

    set DefaultShipmentMethod(value: ShipmentMethod) {
        this.set('DefaultShipmentMethod', value);
    }

    get CanReadResumes(): boolean {
        return this.canRead('Resumes');
    }

    get CanWriteResumes(): boolean {
        return this.canWrite('Resumes');
    }

    get Resumes(): Resume[] {
        return this.get('Resumes');
    }

    AddResume(value: Resume) {
        return this.add('Resumes', value);
    }

    RemoveResume(value: Resume) {
        return this.remove('Resumes', value);
    }

    set Resumes(value: Resume[]) {
        this.set('Resumes', value);
    }

    get CanReadHeadQuarter(): boolean {
        return this.canRead('HeadQuarter');
    }

    get HeadQuarter(): ContactMechanism {
        return this.get('HeadQuarter');
    }


    get CanReadPersonalEmailAddress(): boolean {
        return this.canRead('PersonalEmailAddress');
    }

    get PersonalEmailAddress(): ElectronicAddress {
        return this.get('PersonalEmailAddress');
    }


    get CanReadCellPhoneNumber(): boolean {
        return this.canRead('CellPhoneNumber');
    }

    get CellPhoneNumber(): TelecommunicationsNumber {
        return this.get('CellPhoneNumber');
    }


    get CanReadBillingInquiriesPhone(): boolean {
        return this.canRead('BillingInquiriesPhone');
    }

    get BillingInquiriesPhone(): TelecommunicationsNumber {
        return this.get('BillingInquiriesPhone');
    }


    get CanReadOrderAddress(): boolean {
        return this.canRead('OrderAddress');
    }

    get OrderAddress(): ContactMechanism {
        return this.get('OrderAddress');
    }


    get CanReadInternetAddress(): boolean {
        return this.canRead('InternetAddress');
    }

    get InternetAddress(): ElectronicAddress {
        return this.get('InternetAddress');
    }


    get CanReadContents(): boolean {
        return this.canRead('Contents');
    }

    get CanWriteContents(): boolean {
        return this.canWrite('Contents');
    }

    get Contents(): Media[] {
        return this.get('Contents');
    }

    AddContent(value: Media) {
        return this.add('Contents', value);
    }

    RemoveContent(value: Media) {
        return this.remove('Contents', value);
    }

    set Contents(value: Media[]) {
        this.set('Contents', value);
    }

    get CanReadCreditCards(): boolean {
        return this.canRead('CreditCards');
    }

    get CanWriteCreditCards(): boolean {
        return this.canWrite('CreditCards');
    }

    get CreditCards(): CreditCard[] {
        return this.get('CreditCards');
    }

    AddCreditCard(value: CreditCard) {
        return this.add('CreditCards', value);
    }

    RemoveCreditCard(value: CreditCard) {
        return this.remove('CreditCards', value);
    }

    set CreditCards(value: CreditCard[]) {
        this.set('CreditCards', value);
    }

    get CanReadShippingAddress(): boolean {
        return this.canRead('ShippingAddress');
    }

    get ShippingAddress(): PostalAddress {
        return this.get('ShippingAddress');
    }


    get CanReadCurrentOrganisationContactRelationships(): boolean {
        return this.canRead('CurrentOrganisationContactRelationships');
    }

    get CurrentOrganisationContactRelationships(): OrganisationContactRelationship[] {
        return this.get('CurrentOrganisationContactRelationships');
    }


    get CanReadOpenOrderAmount(): boolean {
        return this.canRead('OpenOrderAmount');
    }

    get OpenOrderAmount(): number {
        return this.get('OpenOrderAmount');
    }


    get CanReadGeneralFaxNumber(): boolean {
        return this.canRead('GeneralFaxNumber');
    }

    get GeneralFaxNumber(): TelecommunicationsNumber {
        return this.get('GeneralFaxNumber');
    }


    get CanReadDefaultPaymentMethod(): boolean {
        return this.canRead('DefaultPaymentMethod');
    }

    get CanWriteDefaultPaymentMethod(): boolean {
        return this.canWrite('DefaultPaymentMethod');
    }

    get DefaultPaymentMethod(): PaymentMethod {
        return this.get('DefaultPaymentMethod');
    }

    set DefaultPaymentMethod(value: PaymentMethod) {
        this.set('DefaultPaymentMethod', value);
    }

    get CanReadCurrentPartyContactMechanisms(): boolean {
        return this.canRead('CurrentPartyContactMechanisms');
    }

    get CurrentPartyContactMechanisms(): PartyContactMechanism[] {
        return this.get('CurrentPartyContactMechanisms');
    }


    get CanReadGeneralPhoneNumber(): boolean {
        return this.canRead('GeneralPhoneNumber');
    }

    get GeneralPhoneNumber(): TelecommunicationsNumber {
        return this.get('GeneralPhoneNumber');
    }


    get CanReadPreferredCurrency(): boolean {
        return this.canRead('PreferredCurrency');
    }

    get CanWritePreferredCurrency(): boolean {
        return this.canWrite('PreferredCurrency');
    }

    get PreferredCurrency(): Currency {
        return this.get('PreferredCurrency');
    }

    set PreferredCurrency(value: Currency) {
        this.set('PreferredCurrency', value);
    }

    get CanReadVatRegime(): boolean {
        return this.canRead('VatRegime');
    }

    get CanWriteVatRegime(): boolean {
        return this.canWrite('VatRegime');
    }

    get VatRegime(): VatRegime {
        return this.get('VatRegime');
    }

    set VatRegime(value: VatRegime) {
        this.set('VatRegime', value);
    }

    get CanReadAmountOverDue(): boolean {
        return this.canRead('AmountOverDue');
    }

    get CanWriteAmountOverDue(): boolean {
        return this.canWrite('AmountOverDue');
    }

    get AmountOverDue(): number {
        return this.get('AmountOverDue');
    }

    set AmountOverDue(value: number) {
        this.set('AmountOverDue', value);
    }

    get CanReadDunningType(): boolean {
        return this.canRead('DunningType');
    }

    get CanWriteDunningType(): boolean {
        return this.canWrite('DunningType');
    }

    get DunningType(): DunningType {
        return this.get('DunningType');
    }

    set DunningType(value: DunningType) {
        this.set('DunningType', value);
    }

    get CanReadAmountDue(): boolean {
        return this.canRead('AmountDue');
    }

    get AmountDue(): number {
        return this.get('AmountDue');
    }


    get CanReadLastReminderDate(): boolean {
        return this.canRead('LastReminderDate');
    }

    get CanWriteLastReminderDate(): boolean {
        return this.canWrite('LastReminderDate');
    }

    get LastReminderDate(): Date {
        return this.get('LastReminderDate');
    }

    set LastReminderDate(value: Date) {
        this.set('LastReminderDate', value);
    }

    get CanReadCreditLimit(): boolean {
        return this.canRead('CreditLimit');
    }

    get CanWriteCreditLimit(): boolean {
        return this.canWrite('CreditLimit');
    }

    get CreditLimit(): number {
        return this.get('CreditLimit');
    }

    set CreditLimit(value: number) {
        this.set('CreditLimit', value);
    }

    get CanReadSubAccountNumber(): boolean {
        return this.canRead('SubAccountNumber');
    }

    get CanWriteSubAccountNumber(): boolean {
        return this.canWrite('SubAccountNumber');
    }

    get SubAccountNumber(): number {
        return this.get('SubAccountNumber');
    }

    set SubAccountNumber(value: number) {
        this.set('SubAccountNumber', value);
    }

    get CanReadBlockedForDunning(): boolean {
        return this.canRead('BlockedForDunning');
    }

    get CanWriteBlockedForDunning(): boolean {
        return this.canWrite('BlockedForDunning');
    }

    get BlockedForDunning(): Date {
        return this.get('BlockedForDunning');
    }

    set BlockedForDunning(value: Date) {
        this.set('BlockedForDunning', value);
    }

    get CanReadDerivationTimeStamp(): boolean {
        return this.canRead('DerivationTimeStamp');
    }

    get CanWriteDerivationTimeStamp(): boolean {
        return this.canWrite('DerivationTimeStamp');
    }

    get DerivationTimeStamp(): Date {
        return this.get('DerivationTimeStamp');
    }

    set DerivationTimeStamp(value: Date) {
        this.set('DerivationTimeStamp', value);
    }


}
