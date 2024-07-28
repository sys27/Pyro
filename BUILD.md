## How to build

### Local Development

#### Prerequisites

1. .NET 8+
2. node.js 22+

#### Build `dotnet` project

Open `Pyro.Api/Pyro.sln` in your favorite IDE or:

1. Navigate to `Pyro.Api`
2. Run `dotnet restore`
3. Run `dotnet build` or `dotnet run`

#### Build `Angular` project

1. Navigate to `Pyro.UI`
2. Run `npm i`
3. Use `npm run start` or `npm run build`

### Docker

Docker creates a single image with .NET application and bundled Angular application.

```bash
docker buildx build -t pyro .
```

## How to run tests

If you want to run all tests, you need to build a docker image of `Pyro`. It is required for integration tests.

### Local Development

To run tests, you can use:

- `dotnet test Pyro.sln` or
- your favorite IDE

### Docker

This command will run tests in a docker container.

```bash
docker buildx build -t tests --target test .
docker run --rm -it -v /var/run/docker.sock:/var/run/docker.sock tests
```

`/var/run/docker.sock` is needed for the `TestContainers` library, it runs the `pyro` image and tests all endpoints against it.

_Note: On Windows/MacOS use `-v /var/run/docker.sock.raw:/var/run/docker.sock` instead._