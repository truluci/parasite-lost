# Parasite Lost - Game Design Document

## Game Overview
A 2D rhythm-based puzzle game where a parasite fragment must navigate through oceanic labyrinths by possessing different fish hosts.

## Core Gameplay

### Parasite Mechanics
- **Lifespan System**: Parasite has a limited lifespan (10 seconds base)
- **Host Possession**: Must possess fish to extend lifespan
- **Rhythm Battles**: Possession happens through rhythm-based combat

### Fish Hosts
- **Small Fish**: Easy rhythm, small lifespan bonus
- **Medium Fish**: Medium rhythm, medium lifespan bonus  
- **Large Fish**: Hard rhythm, large lifespan bonus

### Level Design
- **Labyrinth Structure**: Navigate from point A to B
- **Time Pressure**: Must reach goal within extended lifespan
- **Strategic Possession**: Choose which fish to possess for optimal path

## Audio Design

### Music Tracks
- 1 Background track (ambient oceanic)
- 1 Main menu track
- 3 Rhythm battle tracks (one per fish size)

### Sound Effects
- UI interactions
- Possession sounds
- Environmental ambience
- Rhythm feedback

## Technical Architecture

### Core Systems
- Parasite Controller
- Host Management
- Rhythm System
- Level Management
- Audio Management

### Data-Driven Design
- Fish data via ScriptableObjects
- Level configuration
- Rhythm patterns
- Game progression data
