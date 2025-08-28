import { Component, computed, OnInit, Signal, signal } from '@angular/core';
import { ActivatedRouteSnapshot, ActivationEnd, NavigationEnd, Router, RouterModule, RoutesRecognized } from '@angular/router';
import { filter, map, tap } from 'rxjs';

@Component({
  selector: 'app-navbar-breadcrumb',
  imports: [
    RouterModule,
  ],
  templateUrl: './navbar-breadcrumb.component.html',
  styleUrl: './navbar-breadcrumb.component.scss'
})
export class NavbarBreadcrumbComponent implements OnInit {

  private $router: Router;

  private routeData: any = {};
  breadcrumbs: NavbarBreadcrumb[] = [];

  constructor(
    $router: Router,
  ) {
    this.$router = $router;
  }

  ngOnInit(): void {

    this.$router.events.pipe(
      filter(event => event instanceof RoutesRecognized),
      map(event => event as RoutesRecognized),
    ).subscribe(event => {
      let snapshot: ActivatedRouteSnapshot | null = event.state.root;
      while (snapshot) {
        if (snapshot.data["breadcrumb"]) {
          break;
        }

        snapshot = snapshot.firstChild;
      }
      this.routeData = snapshot?.data ?? {};
    });

    let snapshot: ActivatedRouteSnapshot;
    this.$router.events.pipe(
      tap(event => {
        if (event instanceof ActivationEnd) {
          snapshot = event.snapshot.root;
        }
      }),
      filter(event => event instanceof NavigationEnd),
    ).subscribe(() => {
      try {
        let newBreadcrumbs: NavbarBreadcrumb[] = [];

        let queryParams = snapshot.queryParams;
        const isAppend = "append" in queryParams;
        if (isAppend) {
          const { append, ...queryParamsNoAppend } = queryParams;
          queryParams = queryParamsNoAppend;
        }

        let newBreadcrumb = this.routeData["breadcrumb"] as NavbarBreadcrumb | undefined ?? { link: "/", label: "Home" };
        let newParentBreadcrumbs = this.routeData["parentBreadcrumbs"] as NavbarBreadcrumb[] | undefined ?? [];

        newBreadcrumb = saturateBreadcrumb(newBreadcrumb, snapshot);
        newParentBreadcrumbs = newParentBreadcrumbs.map(item => saturateBreadcrumb(item, snapshot));

        const canonicalBreadcrumbs = [...newParentBreadcrumbs, newBreadcrumb];
        if (canonicalBreadcrumbs[0]?.link !== "/") {
          canonicalBreadcrumbs.unshift({
            label: "Home",
            link: "/",
          });
        }

        const queryBreadcrumbs = queryParams["nav"];
        if (queryBreadcrumbs) {
          try {
            const parsedBreadcrumbs: NavbarBreadcrumb[] | null = decodeNav(queryBreadcrumbs);
            if (!parsedBreadcrumbs) {
              throw new Error();
            }
            parsedBreadcrumbs.push(newBreadcrumb);
            newBreadcrumbs = parsedBreadcrumbs;
          } catch (ex) {
            newBreadcrumbs = canonicalBreadcrumbs;
          }
        } else if (isAppend && this.breadcrumbs.length > 0) {
          newBreadcrumbs = [...this.breadcrumbs];
          newBreadcrumbs.push(newBreadcrumb);
        } else {
          newBreadcrumbs = canonicalBreadcrumbs;
        }

        this.breadcrumbs = newBreadcrumbs;

        if (!queryBreadcrumbs) {
          if (canonicalBreadcrumbs.length > 0 && !navEquals(canonicalBreadcrumbs, this.breadcrumbs)) {
            const paramsWithNav = {
              ...queryParams,
              nav: encodeNav(this.breadcrumbs.slice(0, -1)),
            };
            this.$router.navigate([], { queryParams: paramsWithNav, replaceUrl: true });
          } else {
            if (queryParams["nav"]) {
              const { nav, ...paramsWithoutNav } = queryParams;
              this.$router.navigate([], { queryParams: paramsWithoutNav, replaceUrl: true });
            }
          }
        }
      } catch (ex) {
        console.error(ex);
      }
    });
  }

  safe(input: string | Signal<string> | undefined): Signal<string | undefined> {
    if (typeof input === "string" || !input) {
      return signal(input);
    }

    return input;
  }
}

export interface NavbarBreadcrumb {
  label: string | Signal<string>;
  link: string | Signal<string>;
}

function saturateBreadcrumb(breadcrumb: NavbarBreadcrumb, snapshot: ActivatedRouteSnapshot): NavbarBreadcrumb {
  const result: NavbarBreadcrumb = {
    label: computed(() => {
      const label = typeof breadcrumb.label === "function" ? breadcrumb.label() : breadcrumb.label;
      const params = collectRouteParams(snapshot);
      const saturatedLabel = label.replace(/{(\w+)}/g, (_, key) => params[key] || "");
      return saturatedLabel;
    }),
    link: computed(() => {
      const link = typeof breadcrumb.link === "function" ? breadcrumb.link() : breadcrumb.link;
      const params = collectRouteParams(snapshot);
      const saturatedLink = link.replace(/{(\w+)}/g, (_, key) => params[key] || "");
      return saturatedLink;
    }),
  };

  return result;
}

function collectRouteParams(snapshot: ActivatedRouteSnapshot): Record<string, string> {
  let params: Record<string, string> = {};
  const stack: ActivatedRouteSnapshot[] = [
    snapshot,
  ];

  while (stack.length > 0) {
    const route = stack.pop();
    if (!route) {
      continue;
    }

    params = { ...params, ...route.params };
    stack.push(...route.children);
  }

  return params;
}

function navEquals(computed: NavbarBreadcrumb[], canonical: NavbarBreadcrumb[]) {
  if (canonical.length !== computed.length) {
    return false;
  }

  for (let i = 0; i < canonical.length - 1; i++) {
    if (computed[i].link !== canonical[i].link) {
      return false;
    }
  }

  return true;
}

function encodeNav(input: NavbarBreadcrumb[]): string {
  const data: NavbarBreadcrumb[] = input.map(item => {
    const result: NavbarBreadcrumb = {
      label: typeof item.label === "function" ? item.label() : item.label,
      link: typeof item.link === "function" ? item.link() : item.link,
    };
    return result;
  });
  return btoa(JSON.stringify(data));
}

function decodeNav(input: string): NavbarBreadcrumb[] | null {
  return JSON.parse(atob(input));
}
