# README

## Makefile

- Not needed.

## How to compile (NO NEED TO THOUGH...SKIP)

- Had to compile for linux on my machine because the student network does not have donet/csc programs to build my dotnet console application.

- Here is how you would do it on my machine (Mac M1 arm64)
- Server:
    1. Go to ./A2Receiver.
    2. Run `dotnet publish -r linux-x64 --output ./linux_executable`.
- Client:
    1. Go to ./A2Sender.
    2. Run `dotnet publish -r linux-x64 --output ./linux_executable`.

## How to run

- Fortunately, the executable works on the school's machine.

- In both cases you must specify all the arguments. (2 for ./receiver.sh and 5 for ./sender.sh).
- Specify the correct types (e.g `<port_emulator>` must be a number) otherwise the programs will exit and tell you so.
- emulator:
    0. Run this first.
- receiver:
    0. Run `chmod u+x ./receiver.sh`
    1. Run as `./receiver.sh ....` as usual.
    2. See below for which machine.
- sender:
    0. Run `chmod u+x ./sender.sh`
    1. Run as `./sender.sh ....` as usual.
    2. The `<host_name>` must be the ip-address (e.g `129.97.167.51`) of the school machine.
    3. See below for which machine.

# Which machines
- MUST SPECIFY THE IP-ADDRESS (e.g `129.97.167.51`).
- Tested server on `ubuntu2004-002.student.cs.uwaterloo.ca` (`129.97.167.51`) and `ubuntu2004-004.student.cs.uwaterloo.ca` (e.g `129.97.167.52`).
Tested client on (same as above) `ubuntu2004-002.student.cs.uwaterloo.ca` (`129.97.167.51`) and `ubuntu2004-004.student.cs.uwaterloo.ca` (e.g `129.97.167.52`).
