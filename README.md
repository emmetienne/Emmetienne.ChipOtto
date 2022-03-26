![My Image](./Resources/Icon.png)

![build-and-deploy](https://github.com/emmetienne/Emmetienne.ChipOtto/actions/workflows/deploy.yml/badge.svg)
# ChipOtto
ChipOtto is a C# [Chip-8 Virtual machine](https://en.wikipedia.org/wiki/CHIP-8) implementation, it uses [SDL](https://www.libsdl.org) for showing graphics and getting inputs.

It's a project started for learning purposes to delve into the "low-level" aspects of implementing a Virtual Machine/Emulator for another system.

As now it takes some shortcuts (timing mostly) so it's not 100% accurate but it will run most of the roms for the system.

No roms is provided with the Virtual machine.

## Latest release and download
- [Latest release](https://github.com/emmetienne/Emmetienne.ChipOtto/releases/latest)
- [Download](https://github.com/emmetienne/Emmetienne.ChipOtto/releases/latest/download/ChipOtto.zip)

## Supported systems
- Chip-8
## Inputs
- **F2**: resets the Virtual Machine
- **F4**: closes the Virtual Machine
### Keypad
The Chip-8 keypad has only 16 keys (Hexidecimal 0 to 9, A to F)

|  <!-- -->     |  <!-- -->    |  <!-- -->   | <!-- -->    |
| ---- | ---- | ---- | ---- |
| 1    | 2    | 3    | C    |
| 4    | 5    | 6    | D    |
| 7    | 8    | 9    | E    |
| A    | 0    | B    | F    |

Mapped using the following table

|  <!-- -->     |  <!-- -->    |  <!-- -->   | <!-- -->    |
| ---- | ---- | ---- | ---- |
| 1    | 2    | 3    | 4    |
| Q    | W    | E    | R    |
| A    | S    | D    | F    |
| Z    | X    | C    | V    |

