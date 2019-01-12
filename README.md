# michigames_cabinet_frontend
Frontend game-selection software for the MichiGames Arcade Cabinet.

# Usage
Students traditionally submit information via a lotcheck process like this (https://goo.gl/forms/vdco73yz5aIV86nb2).

This information is then bundled into a data directory that sits alongside the executable as it launches.

The executable will expect to find a directory titled "michigames_games_repo" next to it, containing one subdirectory for each game. Within each game's subdirectory, there must exist...

* screenshots directory
* build.exe
* icon.png
* config.json

## Example
![First-Level of repo](https://i.imgur.com/kC8rrhW.png)
![Second-Level of repo](https://i.imgur.com/GDmz84l.png)
![Screenshots](https://i.imgur.com/qEkcaiB.png)
![config.json contents](https://i.imgur.com/u9YQlD6.png)
