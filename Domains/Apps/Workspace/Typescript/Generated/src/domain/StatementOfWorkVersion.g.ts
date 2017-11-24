// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { SessionObject, Method } from "@allors/base-domain";

import { QuoteVersion } from './QuoteVersion.g';
import { Version } from './Version.g';
import { QuoteState } from './QuoteState.g';
import { QuoteTerm } from './QuoteTerm.g';
import { Party } from './Party.g';
import { ContactMechanism } from './ContactMechanism.g';
import { Currency } from './Currency.g';
import { QuoteItem } from './QuoteItem.g';
import { Request } from './Request.g';
import { StatementOfWork } from './StatementOfWork.g';

export class StatementOfWorkVersion extends SessionObject implements QuoteVersion {
    get CanReadQuoteState(): boolean {
        return this.canRead('QuoteState');
    }

    get CanWriteQuoteState(): boolean {
        return this.canWrite('QuoteState');
    }

    get QuoteState(): QuoteState {
        return this.get('QuoteState');
    }

    set QuoteState(value: QuoteState) {
        this.set('QuoteState', value);
    }

    get CanReadInternalComment(): boolean {
        return this.canRead('InternalComment');
    }

    get CanWriteInternalComment(): boolean {
        return this.canWrite('InternalComment');
    }

    get InternalComment(): string {
        return this.get('InternalComment');
    }

    set InternalComment(value: string) {
        this.set('InternalComment', value);
    }

    get CanReadRequiredResponseDate(): boolean {
        return this.canRead('RequiredResponseDate');
    }

    get CanWriteRequiredResponseDate(): boolean {
        return this.canWrite('RequiredResponseDate');
    }

    get RequiredResponseDate(): Date {
        return this.get('RequiredResponseDate');
    }

    set RequiredResponseDate(value: Date) {
        this.set('RequiredResponseDate', value);
    }

    get CanReadValidFromDate(): boolean {
        return this.canRead('ValidFromDate');
    }

    get CanWriteValidFromDate(): boolean {
        return this.canWrite('ValidFromDate');
    }

    get ValidFromDate(): Date {
        return this.get('ValidFromDate');
    }

    set ValidFromDate(value: Date) {
        this.set('ValidFromDate', value);
    }

    get CanReadQuoteTerms(): boolean {
        return this.canRead('QuoteTerms');
    }

    get CanWriteQuoteTerms(): boolean {
        return this.canWrite('QuoteTerms');
    }

    get QuoteTerms(): QuoteTerm[] {
        return this.get('QuoteTerms');
    }

    AddQuoteTerm(value: QuoteTerm) {
        return this.add('QuoteTerms', value);
    }

    RemoveQuoteTerm(value: QuoteTerm) {
        return this.remove('QuoteTerms', value);
    }

    set QuoteTerms(value: QuoteTerm[]) {
        this.set('QuoteTerms', value);
    }

    get CanReadValidThroughDate(): boolean {
        return this.canRead('ValidThroughDate');
    }

    get CanWriteValidThroughDate(): boolean {
        return this.canWrite('ValidThroughDate');
    }

    get ValidThroughDate(): Date {
        return this.get('ValidThroughDate');
    }

    set ValidThroughDate(value: Date) {
        this.set('ValidThroughDate', value);
    }

    get CanReadDescription(): boolean {
        return this.canRead('Description');
    }

    get CanWriteDescription(): boolean {
        return this.canWrite('Description');
    }

    get Description(): string {
        return this.get('Description');
    }

    set Description(value: string) {
        this.set('Description', value);
    }

    get CanReadReceiver(): boolean {
        return this.canRead('Receiver');
    }

    get CanWriteReceiver(): boolean {
        return this.canWrite('Receiver');
    }

    get Receiver(): Party {
        return this.get('Receiver');
    }

    set Receiver(value: Party) {
        this.set('Receiver', value);
    }

    get CanReadFullfillContactMechanism(): boolean {
        return this.canRead('FullfillContactMechanism');
    }

    get CanWriteFullfillContactMechanism(): boolean {
        return this.canWrite('FullfillContactMechanism');
    }

    get FullfillContactMechanism(): ContactMechanism {
        return this.get('FullfillContactMechanism');
    }

    set FullfillContactMechanism(value: ContactMechanism) {
        this.set('FullfillContactMechanism', value);
    }

    get CanReadPrice(): boolean {
        return this.canRead('Price');
    }

    get CanWritePrice(): boolean {
        return this.canWrite('Price');
    }

    get Price(): number {
        return this.get('Price');
    }

    set Price(value: number) {
        this.set('Price', value);
    }

    get CanReadCurrency(): boolean {
        return this.canRead('Currency');
    }

    get CanWriteCurrency(): boolean {
        return this.canWrite('Currency');
    }

    get Currency(): Currency {
        return this.get('Currency');
    }

    set Currency(value: Currency) {
        this.set('Currency', value);
    }

    get CanReadIssueDate(): boolean {
        return this.canRead('IssueDate');
    }

    get CanWriteIssueDate(): boolean {
        return this.canWrite('IssueDate');
    }

    get IssueDate(): Date {
        return this.get('IssueDate');
    }

    set IssueDate(value: Date) {
        this.set('IssueDate', value);
    }

    get CanReadQuoteItems(): boolean {
        return this.canRead('QuoteItems');
    }

    get CanWriteQuoteItems(): boolean {
        return this.canWrite('QuoteItems');
    }

    get QuoteItems(): QuoteItem[] {
        return this.get('QuoteItems');
    }

    AddQuoteItem(value: QuoteItem) {
        return this.add('QuoteItems', value);
    }

    RemoveQuoteItem(value: QuoteItem) {
        return this.remove('QuoteItems', value);
    }

    set QuoteItems(value: QuoteItem[]) {
        this.set('QuoteItems', value);
    }

    get CanReadQuoteNumber(): boolean {
        return this.canRead('QuoteNumber');
    }

    get CanWriteQuoteNumber(): boolean {
        return this.canWrite('QuoteNumber');
    }

    get QuoteNumber(): string {
        return this.get('QuoteNumber');
    }

    set QuoteNumber(value: string) {
        this.set('QuoteNumber', value);
    }

    get CanReadRequest(): boolean {
        return this.canRead('Request');
    }

    get CanWriteRequest(): boolean {
        return this.canWrite('Request');
    }

    get Request(): Request {
        return this.get('Request');
    }

    set Request(value: Request) {
        this.set('Request', value);
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
