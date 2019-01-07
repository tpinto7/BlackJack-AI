# Summary
Recreated the popular betting game into a console, text-based simulator with a smart AI that maximizes its odds of winning. Allows for up to 8 players, human or computer, to challenge the dealer.

# Game Play
The game progresses as a series of betting rounds. Players first place their bets, then receive two cards each. They can then surrender, hit, double, or pass as they see fit based on the dealer's card. A useful guide was added that indicates to the player their odds of busting if they hit. Once all players have acted, the dealer hits until they attain a hand value of at least 17. Finally, the values of the players are compared to the dealer, resulting in appropriate losses or gains to the player's coin pool. The game can be concluded after any round and a player is eliminated if their initial stack of 100 coins dwindle to zero.

# AI
There are two levels of AI in this program. The first uses a standard formula for hitting, doubling, passing, etc. based on its current hand value, as well as the dealer's up-card. The second AI exhaustively considers all possible hand combinations based on the remaining cards in the deck. It determines the likelihood of each possible outcome for the dealer's hand. Then, it compares its ratio of winning if passes or hits based on all remaining possible card combinations. Based of this ratio it will also determine if it should double or surrender. 
