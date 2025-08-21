# 🧦 Sock Sorter VR  

**Sock Sorter VR** is an immersive virtual reality game where players grab and sort socks into the correct baskets.  
The game uses **gesture-based locomotion** for natural movement and **object grabbing mechanics** for sock interaction.  

---

## ✨ Features  

- **Gesture-Based Locomotion**  
  - Left-hand 👍 → Move Forward  
  - Right-hand 👍 → Move Backward  

- **Sock Spawning & Tracking**  
  - Socks spawn dynamically.  
  - Each sock is validated with its unique color/type.  

- **Sorting & Validation**  
  - Place socks into their designated baskets.  
  - Validation logic checks correctness.  
  - Once placed, socks cannot be grabbed again during validation.  

- **Immersive Interaction**  
  - Built with **Unity XR Interaction Toolkit** (or compatible VR framework).  
  - Realistic grabbing & placement.  

---

## 🎮 Gameplay Flow  

1. A new sock spawns in the scene.  
2. The UI shows the name/type of the spawned sock (e.g., *Red Sock*).  
3. Player moves around the environment using **thumb-up gestures**.  
4. Player grabs the sock and places it inside the correct basket.  
5. During placement validation, the sock cannot be grabbed again.  
6. Sock is destroyed (or handled accordingly) once validation is complete.  

---

## 🛠️ Tech Stack  

- **Engine**: Unity (XR Interaction Toolkit, Hand Tracking)  
- **Language**: C#  
- **Platform**: VR (tested with OpenXR-compatible headsets)  

---

## 📂 Project Structure  

/Scripts
- SpawnManager.cs # Handles spawning of socks
- SockTracker.cs # Tracks spawned socks
- SockValidator.cs # Validates sock correctness
- PlayerMovement.cs # Gesture-based locomotion
- BasketHandler.cs # Disables grabbing after placement
/Prefabs
- Socks (Red, Blue, Striped, etc.)
- Baskets

---

## 🚀 Getting Started  

1. Clone this repository:  
   ```bash
   git clone https://github.com/your-username/sock-sorter-vr.git
2. Open in Unity Hub.
3. Set up XR Plug-in Management for your VR headset.
4. Press ▶️ to run inside the Unity Editor or build to your VR device.

---
