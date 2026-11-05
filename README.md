# Arcfire

**Arcfire** is a multiplayer third-person shooter developed in Unity as part of my university Game Networking and Security coursework. The project focuses on multiplayer networking, synchronised gameplay systems, player progression, and backend integration.

Built using Photon PUN for real-time multiplayer networking and PlayFab for player authentication and persistent leaderboard data, the project explores the challenges of building a networked game with synchronised gameplay and supporting services.

## Features

### Multiplayer Gameplay
- Real-time multiplayer using Photon PUN.
- Third-person movement with camera-relative controls.
- Cinemachine third-person camera system, independently following each player's character.
- Synchronised weapon aiming, including vertical angle adjustment.
- Elevated platforms introducing verticality to combat.
- Collision filtering to prevent players from damaging themselves.

### Gameplay Systems
- Centralised GameManager for managing game state, player references, spawning locations, and cursor states.
- Multiplayer health system with locally displayed HUD health bars and synchronised world-space health bars.
- Match timer and point-based win conditions.
- End-game system supporting multiple winners in the event of a tied score.
- Lobby and rematch functionality.

### Backend Integration & Persistence
- PlayFab integration for player identity and leaderboard functionality.
- Asynchronous username synchronisation before establishing a Photon connection.
- Persistent local save data protected using JSON serialisation, Base64 encoding, and AES encryption.
- Weekly leaderboard tracking using PlayFab statistics with SUM aggregation.
- Retry mechanism for failed score submissions.

### Administrative Command System
Implemented an extensible command system using the Command Pattern, separating command parsing from execution.

The system uses an `IGameCommand` interface and a central `CommandDispatcher` to register and execute commands.

Available commands:

| Command | Description |
|---|---|
| `Heal` | Restore a player's health to maximum. |
| `Time` | Modify the remaining match time. |
| `Kick` | Remove a player from the game. |
| `Quit` | Exit the game. |
| `Pause` | Pause or resume the match timer. |
| `Add Point` | Award a point to a specified player. |

## Technical Implementation

Some of the key engineering decisions made during development:

**Asynchronous Authentication**

PlayFab's callback-based API was integrated with C#'s `TaskCompletionSource` pattern, allowing asynchronous username updates to be awaited before initiating the Photon connection. This ensures that player identity is synchronised before entering the multiplayer environment.

**Network Synchronisation**

Photon's ownership system (`photonView.IsMine`) is used to ensure each client controls its own character and camera. Weapon pivot movement is synchronised across clients using `PhotonTransformView`.

**Game State Management**

A centralised GameManager handles shared game state and player management. Its execution order was adjusted to ensure initialisation occurs before dependent systems access it.

**Extensible Command Architecture**

The Command Pattern allows administrative functionality to be extended without modifying the central input parsing logic. Each command implements the same interface and is registered with the dispatcher.

## Technologies

- **Engine:** Unity
- **Language:** C#
- **Networking:** Photon PUN
