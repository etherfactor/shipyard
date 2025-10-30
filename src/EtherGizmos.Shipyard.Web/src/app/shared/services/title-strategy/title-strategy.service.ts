import { Injectable } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { TitleStrategy, RouterStateSnapshot, ActivatedRouteSnapshot } from "@angular/router";

@Injectable()
export class TitleStrategyService extends TitleStrategy {
  constructor(private readonly title: Title) {
    super();
  }

  override updateTitle(routerState: RouterStateSnapshot) {
    let title = this.buildTitle(routerState);
    if (title !== undefined) {
      title = this.processParams(routerState.root, title);
      this.title.setTitle(title);
    }
  }

  private processParams(snapshot: ActivatedRouteSnapshot, title: string): string {
    for (const [key, value] of Object.entries(snapshot.params)) {
      title = title.replaceAll(`:${key}`, value);
    }

    for (const child of snapshot.children) {
      title = this.processParams(child, title);
    }

    return title;
  }
}
