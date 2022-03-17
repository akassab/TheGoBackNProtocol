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


## Example
- Emulator use `129.97.167.51` (ubuntu2004-002)
- Receiver use `129.97.167.52` (ubuntu2004-004)
- Sender use  `129.97.167.27` (ubuntu2004-008)
- Run in this order:
### On Emualtor Machine
- `cd cs456-a2-nemulator`
- `chmod u+x ./nEmulator`
-  `./nEmulator 9991 129.97.167.52 9994 9993 129.97.167.27 9992 1 0.2 0`
### On Receiver Machine
- `chmod u+x ./receiver.sh`
- `./receiver.sh 129.97.167.51 9993 9994 output_file.txt`
### On Sender Machine
- `chmod u+x ./sender.sh`
- `./sender.sh 129.97.167.51 9991 9992 50 ./tests/2_lap_65_packets.txt`
### More test files for the sender:
- 0_lap_31_packets.txt (should send 30 data + 1 eot packet(s))
- 1_lap_32_packets.txt (should send 31 data + 1 eot packet(s) and make a lap)
- 1_lap_33_packets.txt (should send 32 data + 1 eot packet(s) and make a lap)
- 1_lap_64_packets.txt (should send 63 data + 1 eot packet(s) and make a lap)
- 2_lap_65_packets.txt (should send 64 data + 1 eot packet(s) and make two laps)
# Which machines
- MUST SPECIFY THE IP-ADDRESS (e.g `129.97.167.51`).
- Tested server on `ubuntu2004-002.student.cs.uwaterloo.ca` (`129.97.167.51`) and `ubuntu2004-004.student.cs.uwaterloo.ca` (e.g `129.97.167.52`).
Tested client on (same as above) `ubuntu2004-002.student.cs.uwaterloo.ca` (`129.97.167.51`) and `ubuntu2004-004.student.cs.uwaterloo.ca` (e.g `129.97.167.52`).
