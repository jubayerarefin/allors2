import { ObjectType, PropertyType, MetaObjectType } from '../../meta';
import { Tree } from './Tree';
import { Step } from './Step';

const includeKey = 'include';

export class Fetch {
  public step: Step;

  public include: Tree;

  constructor(fields?: Partial<Fetch> | ObjectType | MetaObjectType, literal?) {

    if (fields instanceof ObjectType || fields && (fields as MetaObjectType)._objectType) {
      const objectType = (fields as MetaObjectType)._objectType ? (fields as MetaObjectType)._objectType : fields as ObjectType;

      if (literal) {
        const keys = Object.keys(literal);

        if (keys.find(v => v === includeKey)) {
          const treeLiteral = literal[includeKey];
          this.include = new Tree(objectType, treeLiteral);
        }

        if (keys.length > 0) {
          const stepName = keys.find(v => v !== includeKey);
          const stepLiteral = literal[stepName];
          this.step = new Step(objectType, stepName, stepLiteral);
        }
      }

    } else {
      Object.assign(this, fields);
    }
  }

  public toJSON(): any {

    return {
      step: this.step,
      include: this.include,
    };
  }
}