import { MetaPopulation, Workspace, PullRequest } from "./framework";
import { Database, Context } from "./promise";
import { AxiosHttp } from "./promise/core/http/AxiosHttp";
import { data, Meta, PullFactory } from "./meta";
import { domain } from './domain';
import { NodeMapper } from "./gatsby/NodeMapper";

import { NodePluginArgs } from "gatsby"

export class NodeBuilder {

  http: AxiosHttp;
  metaPopulation: MetaPopulation;
  m: Meta;
  workspace: Workspace;
  mapper: NodeMapper;
  login: string;
  user: string;
  password: string;

  constructor(args: NodePluginArgs, { plugins, ...options }) {
    const url = options["url"]
    this.http = new AxiosHttp(url);
    this.login = options["login"];
    this.user = options["user"];
    this.password = options["password"];

    this.metaPopulation = new MetaPopulation(data);
    this.m = this.metaPopulation as Meta;

    this.workspace = new Workspace(this.metaPopulation);
    domain.apply(this.workspace);

    this.mapper = new NodeMapper(args, options);
  }

  public async build() {

    // Meta Gatsby
    this.m.Organisation.metaGatsby = {
      roleTypes: this.m.Organisation.roleTypes,
      properties: ["slug"]
    };

    this.m.Person.metaGatsby = {
      roleTypes: this.m.Person.roleTypes,
      associationTypes: this.m.Person.associationTypes,
      properties: ["slug"]
    };

    this.m.Media.metaGatsby = {
      roleTypes: this.m.Media.roleTypes,
    };

    // Fetch objects
    await this.http.login(this.login, this.user, this.password);

    const database = new Database(this.http);
    const ctx = new Context(database, this.workspace);
    const pull = new PullFactory(this.m);

    const pulls = [
      pull.Organisation({
        include: {
          Owner: {},
          Employees: {},
        }
      }),
      pull.Person(),
      pull.Media(),
    ];

    ctx.session.reset();

    const loaded = await ctx
      .load(new PullRequest({ pulls }));

    // Map to Nodes
    this.mapper.map(loaded);
  }
}
