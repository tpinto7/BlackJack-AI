# BlackJack-AI
Recreated the popular betting game with a smart AI that maximizes its odds of winning. Allows for up to 8 players, human or computer, to challenge the dealer.

# AI
There are two levels of AI in this program. The first uses a standard formula for hitting, doubling, passing, etc. based on its current hand value,
as well as the dealer's up-card. The second AI exhaustively considers all possible hand combinations based on the remaining cards in the deck.
It determines the likelihood of each possible outcome for the dealer's hand. Then, it compares its ratio of winning if passes or hits based on all remaining
possible card combinations. Based of this ratio it will also determine if it should double or surrender. 
