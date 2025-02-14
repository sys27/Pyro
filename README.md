# Pyro

[![Build Docker Image CI](https://github.com/sys27/Pyro/actions/workflows/docker-image.yml/badge.svg)](https://github.com/sys27/Pyro/actions/workflows/docker-image.yml)
![GitHub License](https://img.shields.io/github/license/sys27/Pyro)

Pyro is a platform designed to manage and collaborate on code, repositories, issues, and pull requests.

## Features:

- [ ] Repository Management
  - [x] Manage git repositories (create/update)
  - [ ] Browse commit/branches/files in UI
  - [x] Clone/push over HTTP/HTTPS
  - [ ] Clone/push over SSH
- [ ] Issues Tracking and Management
  - [x] Manage issues (create/update/delete)
  - [x] Statuses, Transition rules
  - [x] Labels
  - [ ] Releases
  - [x] Lock issues
  - [x] Track change history
  - [ ] Track issues in commits (on push, merge, etc)
- [ ] Pull Requests
- [ ] Users and Permissions Management
  - [x] Manage users (create/update)
  - [ ] Manage roles/permissions
  - [ ] Apply permissions to application
- [ ] Pipelines
- [ ] Host as Docker Container

## How to run

```bash
docker run -d \
    --name pyro \
    -p 8080:80 \
    -v $(pwd)/data:/data \
    sys27/pyro:latest
```

`Pyro` stores all information (a database file and repositories) in the `/data` directory of the container. And by default, it creates an anonymous volume but you can create your volume and mount it into `/data` or use bind mounts.

Now you can navigate to `http://localhost:8080/` and login with `pyro`/`pyro`.

## How to build / run tests

See [BUILD.md](https://github.com/sys27/Pyro/blob/master/BUILD.md)

## License

xFunc is released under [GNU GPL 3.0](https://github.com/sys27/Pyro/blob/master/LICENSE).