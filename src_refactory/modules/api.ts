class ApiNotFound extends Error {
    constructor(message: string) {
      super(message);
      this.name = "ApiNotFound";
    }
}
class ApiErrorGeneric extends Error {
    constructor(message: string) {
      super(message);
      this.name = "ApiErrorGeneric";
    }
}
class ApiConflict extends Error {
    constructor(message: string) {
      super(message);
      this.name = "ApiConflict";
    }
}
class ApiUnauthorized extends Error {
    constructor(message: string) {
      super(message);
      this.name = "ApiUnauthorized";
    }
}

export {
    ApiNotFound,
    ApiErrorGeneric,
    ApiConflict,
    ApiUnauthorized
}