import { Meta } from '../allors/meta';
import { MetaPopulation, And, Like } from '../allors/framework';
import { FilterDefinition } from '../allors/angular';
import { Sorter } from '../allors/material/core/sorting';

export function appMeta(metaPopulation: MetaPopulation) {
  const m = metaPopulation as Meta;

  m.Person.list = '/contacts/people';
  m.Person.overview = '/contacts/person/:id';
  m.Person.filterDefinition = new FilterDefinition(
    new And([
      new Like({ roleType: m.Person.FirstName, parameter: 'firstName' }),
      new Like({ roleType: m.Person.LastName, parameter: 'lastName' }),
      new Like({ roleType: m.Person.UserEmail, parameter: 'email' }),
    ]),
  );
  m.Person.sorter = new Sorter({
    firstName: m.Person.FirstName,
    lastName: m.Person.LastName,
    email: m.Person.UserEmail,
  });

  m.Organisation.list = '/contacts/organisations';
  m.Organisation.overview = '/contacts/organisation/:id';
  m.Organisation.filterDefinition = new FilterDefinition(new And([new Like({ roleType: m.Organisation.Name, parameter: 'name' })]));
  m.Organisation.sorter = new Sorter({ name: m.Organisation.Name });
}
