# **Treasure Hunt AR!**

## **Description**

Treasure Hunt AR is an **augmented reality (AR) game** built with **Unity & AR Foundation** that turns any flat real-world surface — a floor, table, or desk — into a virtual ocean. A pirate ship sails across the detected AR surface to collect treasure chests while avoiding and battling krakens.

The game demonstrates practical mobile AR development: real-time plane detection, screen-space raycasting onto detected surfaces, persistent object placement on AR planes, physics-driven interaction, and a full in-game UI.

## **Getting Started**

### **Prerequisites**

To run this project, you'll need:

- **Unity** (AR Foundation compatible, 2021.3.11f1 or newer)
- **Visual Studio Code** (with the Unity extension installed)
- An **ARCore-supported Android device**

### **How to Play**

1. **Launch the game.**
2. **Grant camera permissions** when prompted.
3. **Scan your surroundings** to detect a flat surface — the arena locks to the first surface found and grows as you scan more.
4. **Tap the screen** to spawn your ship.
5. **Point your camera** to steer the ship toward treasure chests and collect as much as possible before the timer expires.
6. **Beware of the Kraken!** Some chests are guarded — ram the kraken to defeat it (you lose 1 HP per hit) and unlock the chest.
7. You have **3 HP** and a short time limit. If you run out of HP or time, the game ends.

Set sail and see how much treasure you can claim before time runs out! 🏴‍☠️✨

## **Features**

- AR floor detection with a persistent, infinitely growing playable platform
- Camera-steered ship movement (screen centre acts as steering target)
- Automatic treasure chest spawning (up to 3 at a time)
- Kraken guarding system with combat and HP mechanics
- Ship HP, score counter, and countdown timer HUD
- Reliable hit detection immune to AR height drift
- Splash screen, step-by-step onboarding, and game-over scoreboard with Play Again

## **Developers**

- **Yuvam Tougani**
- **Arjun Manoj**
- **Samith Sanka**
- **Nikhil Thomas**
