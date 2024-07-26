# Tests

To run the tests for checkpoints use the following commands

Checkpoint 01

> `dotnet test t1`

Checkpoint 02

> `dotnet test t2`

Checkpoint 03

> `dotnet test t3`

Checkpoint 04

> `dotnet test t4`

Checkpoint 05

> `dotnet test t5`

Commands above works when your working directory is the `tests` directory.

If you want to run the tests from another working directory then use command `dotnet test [path]` where [path] is replaced with path to a test project folder (e.g. `dotnet test tests\t1`).

## To run all the tests

When using Visual Studio, run the tests from the Test Explorer.

When using terminal (or command line) run command

```dotnetcli
dotnet test
```

when the working directory is the directory where the solution file (*.sln) is located.

