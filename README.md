# FrameKill
Created by Mao (@NZMao)

This program aims to help in calculating potential framekill options given a set of moves

Source files should be in the .csv format and contain two columns
- Column 1 contains the move names
- Column 2 contains the amount of frames each move takes up

The program calculates a sequence of moves that will take up an amount of frames close to the desired amount

Depending on the source file, not all timings will be possible.
Therefore, it is possible to set the desired value as an upper bound or a lower bound:

Upper Bound:
- Will attempt to find a sequence that maximises the time taken while being equal to or lower than the desired value

Lower Bound:
- Will attempt to find a sequence that minimises the time taken while being equal to or greater than the desired value

Some example .csv files are included to get some grasp of the format required, this theoretically can be made to suit any fighting game
