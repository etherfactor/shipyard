/* eslint-disable @typescript-eslint/ban-types */
/* eslint-disable prefer-rest-params */

/**
 * Binds an instance method to the containing class to persist the lexical scope of 'this'.
 * @param target The target class or prototype; used by the TypeScript compiler (omit function call brackets to use as a decorator).
 * @param propKey The property key of the target method; used by the TypeScript compiler (omit function call brackets to use as a decorator).
*/
export function Bound(target: object, propKey: string | symbol): PropertyDescriptor {
  const originalMethod = target[propKey as keyof object] as Function;

  // Ensure the above type-assertion is valid at runtime.
  if (typeof originalMethod !== "function") throw new TypeError("@Bound can only be used on methods.");

  if (typeof target === "function") {
    // Static method, bind to class (if target is of type "function", the method decorator was used on a static method).
    return {
      value: function () {
        return originalMethod.apply(target, arguments);
      }
    } as PropertyDescriptor;
  } else if (typeof target === "object") {
    // Instance method, bind to instance on first invocation (as that is the only way to access an instance from a decorator).
    return {
      get: function () {
        // Create bound override on object instance. This will hide the original method on the prototype, and instead yield a bound version from the
        // instance itself. The original method will no longer be accessible. Inside a getter, 'this' will refer to the instance.

        // eslint-disable-next-line @typescript-eslint/no-this-alias
        const instance = this;

        Object.defineProperty(instance, propKey.toString(), {
          value: function () {
            // This is effectively a lightweight bind() that skips many (here unnecessary) checks found in native implementations.
            return originalMethod.apply(instance, arguments);
          }
        });

        // The first invocation (per instance) will return the bound method from here. Subsequent calls will never reach this point, due to the way
        // JavaScript runtimes look up properties on objects; the bound method, defined on the instance, will effectively hide it.
        return instance[propKey as keyof PropertyDescriptor];
      }
    } as PropertyDescriptor;
  } else {
    throw new Error(`Unexpected typeof target: ${typeof target}`);
  }
}
