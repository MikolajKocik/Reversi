# Reversi

A classic strategy board game application built with Windows Presentation Foundation (WPF).

## Table of Contents

- [Overview](#overview)
- [System Requirements](#system-requirements)
- [Game Features](#game-features)
- [Game Rules](#game-rules)
- [Computer AI Strategy](#computer-ai-strategy)
- [User Interface](#user-interface)
- [How to Play](#how-to-play)
- [Technical Specifications](#technical-specifications)
- [Author](#author)

## Overview

Reversi is a two-player strategy board game where players take turns placing colored pieces on a board. The objective is to have the majority of pieces showing your color when the game ends. This application provides both single-player (against computer AI) and two-player modes with customizable board dimensions.

## System Requirements

- **Operating System**: Windows 7 or later
- **Framework**: .NET Framework 4.7.2 or later
- **Platform**: Windows desktop (x86, x64, or AnyCPU)

## Game Features

### Game Modes

1. **Single Player Mode**
   - Play against an intelligent computer opponent
   - Choose who starts first:
     - Computer starts (plays as Brown)
     - Player starts (plays as Green)

2. **Two Player Mode**
   - Local multiplayer for two human players
   - Green player always moves first

### Customizable Board

- Configure board dimensions before starting a new game
- Width: 4 to 26 fields
- Height: 4 to 26 fields
- Default board size: 8×8 (standard Reversi/Othello board)

### Assistance Features

- **Move Hint**: Highlights the best available move for the current player
- **Computer Move**: Executes the optimal move automatically (useful for learning or assistance)
- **Quick Access**: Click the player color button for a move hint, or hold Ctrl and click for an automatic move

### Score Tracking

- Real-time display of piece counts for both players
- Move history lists for Green and Brown players
- Visual indication of current player's turn

## Game Rules

Reversi follows the classic rules of the strategy board game:

1. **Objective**: Capture more fields than your opponent by the end of the game.

2. **Starting Position**: The game begins with four pieces placed in the center of the board in an alternating pattern.

3. **Making Moves**:
   - Players take turns placing their pieces on empty fields
   - A move is only valid if it captures at least one opponent's piece
   - Pieces are captured by surrounding opponent's pieces between your new piece and any existing pieces of your color
   - Captures can occur in all eight directions (horizontal, vertical, and diagonal)

4. **Passing**:
   - If a player has no valid moves, they must pass their turn
   - The other player then continues making moves

5. **Game End Conditions**:
   - All fields on the board are occupied
   - Neither player can make a valid move
   - The player with more pieces wins; equal pieces result in a draw

## Computer AI Strategy

The computer opponent uses a priority-based decision system to determine moves:

| Priority | Strategy |
|----------|----------|
| Highest | Occupy corner positions (strategic advantage) |
| High | Prefer edge positions |
| Neutral | Maximize captured pieces |
| Low | Avoid positions adjacent to edges |
| Lowest | Avoid positions adjacent to corners (gives opponent corner access) |

The AI evaluates all possible moves and assigns priority scores based on:
- Position value (corners, edges, central areas)
- Number of pieces that would be captured
- Strategic positioning to prevent opponent advantages

## User Interface

### Main Window

- **Game Board**: Interactive grid displaying the current game state
- **Player Indicator**: Button showing the current player's color
- **Score Display**: Real-time piece counts for Green and Brown players
- **Move History**: Separate lists tracking moves for each player

### Menu Options

#### Game Menu
- **New single player game**
  - Computer starts (brown)
  - You start (green)
- **New two player game**
- **Exit**

#### Help Menu
- **Move hint**: Shows the recommended move
- **Computer move**: Executes an AI-calculated move
- **Game rules**: Displays the rules of Reversi
- **Computer strategy**: Explains the AI decision-making process
- **About**: Application information

### Visual Elements

- **Green pieces**: Player 1 / Human player (when playing against computer)
- **Brown pieces**: Player 2 / Computer opponent
- **Beige fields**: Empty board positions
- **Highlighted field**: Suggested move hint (blended color)

## How to Play

### Starting a Game

1. Launch the application
2. Select **Game** from the menu
3. Choose your preferred game mode:
   - Single player: Select whether you or the computer plays first
   - Two players: Start a local multiplayer game
4. In the dialog that appears, optionally adjust the board dimensions
5. Click **OK** to begin

### Making a Move

1. The current player is indicated by the colored button in the top-right area
2. Click any valid field on the board to place your piece
3. Invalid moves will have no effect
4. Captured opponent pieces automatically flip to your color

### Using Assistance

- Select **Help > Move hint** to see the best available move
- Select **Help > Computer move** to have the AI make a move for you
- Alternatively, click the player color button for hints or Ctrl+Click for automatic moves

### End of Game

- When the game ends, a message displays the winner or declares a draw
- You will be prompted to start a new game or close the application

## Technical Specifications

### Architecture

| Component | Description |
|-----------|-------------|
| **MainWindow** | Primary user interface and game coordination |
| **NewGameDialog** | Board size configuration dialog |
| **ReversiEngine** | Core game logic, rules validation, and state management |
| **ReversiEngineAI** | AI opponent with strategic move calculation |
| **ColorBlending** | Visual effects for move hint highlighting |

### Technology Stack

- **Framework**: .NET Framework 4.7.2
- **UI Technology**: Windows Presentation Foundation (WPF)
- **Language**: C#
- **Architecture Pattern**: Event-driven with separation of game logic

### Build Configuration

- Debug and Release configurations available
- Target platform: AnyCPU (compatible with x86 and x64 systems)
- Output type: Windows executable (WinExe)
