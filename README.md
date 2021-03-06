﻿Genetic Algorithm

NOTES:
- Crossover rate is 100% (1-point crossover)
- Population shrinks over time (remove baby_amount from DNA and always create population of initial size?)
- Time elapsed is not 1 seconds accurate
- Create object-pool to improve performance with high value populations

Creatures:
Each creature has the following properties:
- Health: The amount of health it starts with, when this reaches 0 the creature will die. (this is the Fitness)
- Maximum capacity: This specifies how much food the creature can hold in its stumach.
- Digest time: How fast will 1 food digest from the stumach and change into 1 health point. (in seconds)
- Eye sight: How far the creature can see and spot food or obstacles.
- Baby amount: How much babies this parent can make, the amount between 2 parents is randomly chosen. (min: 1, max: 3)
- Speed: speed at which the creature moves.
The creatures are supposed to look for food so that they will survive for a fixed amount of time. 
Health drops down with 1 after X amount of time.

World:
The world has the following properties:
- Width: Width of the world.
- Height: Height of the world.
- Food: Food that creatures can eat.

Generation:
Each generation has to survive for X amount of time. After that time has passed a group with the highest health is selected for mating.
After mating X new creatures will be created (X = Creature.Baby_amount).
Within this creation there are 2 fixed properties:
- Crossover rate = 80%-95%
- Mutation rate = 0.5%-1%
	- 33% chance of negative mutation
	- 66% chance of positive mutation
After the creation the generation will follow the same iteration as their parents.
