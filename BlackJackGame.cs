using System;
using static System.Console;

using System.Collections.Generic;
using BlackJackDeck;
using BlackJackPlayer;

namespace Game
{
	class BlackJackGame : Player
	{
		private const int cardsPerPlayer = 2; 
		List< Player > players; 
		List< Player > splits;
		Player dealer;
		
		public Deck gameCards;
		public Deck gameCardsVisible; 
		public Deck gameCardsUsed;
		
		// override player class to string and replace player name with it
		public int NumberOfPlayers { get; set; }
		public int NumberOfAi { get; set; }
		public int NumberOfSmartAi{ get; set; }
		public string[ ] playerNames;
		public int[ ] playerCoins;
		public int[ ] currentBets; 
		public int[ ] insuranceBets;
		
		public BlackJackGame( )
		{
			gameCards = new Deck( );
			gameCards.FillDeck( );
			gameCardsVisible = gameCards;
			gameCardsUsed = new Deck( );
			
			players = new List< Player >( );
			splits = new List< Player >( );
			dealer = new Player( );
		}
		
		public void CreatePlayers( )
		{
			playerNames = new string[ NumberOfPlayers ];
			playerCoins = new int[ NumberOfPlayers ];
			currentBets = new int[ NumberOfPlayers ];
			insuranceBets = new int[ NumberOfPlayers ];
			
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				players.Add( new Player( ) );
				splits.Add( new Player( ) );
				
				Write( $"Enter if Player {i+1} is 'human', 'ai', or 'smart ai': ");
				string humanOrAi = ReadLine( );
				
				while( true )
				{
					if( humanOrAi.Equals( "human" ) )
					{
						players[ i ].IsHuman = true; 
						break;
					}
					else if( humanOrAi.Equals( "ai" ) )
					{
						players[ i ].IsHuman = false;
						playerNames[ i ] = $"Ai {++NumberOfAi}";
						break;
					}
					else if( humanOrAi.Equals( "smart ai" ) )
					{
						players[ i ].IsHuman = false;
						playerNames[ i ] = $"Smart Ai {++NumberOfSmartAi}";
						break;
					}
					else
					{
						Write( $"Enter if Player {i+1} is 'human' or 'ai': ");
						humanOrAi = ReadLine( );
					}
				}
				
				if( players[ i ].IsHuman )
				{
					Write( $"Enter Player {i+1}'s Name: " );
					playerNames[ i ] = ReadLine( ).Trim( ); 
					
					bool nameCheck = true;
					while( nameCheck ) 
					{
						nameCheck = false;
						for( int j = 0; j < i; j ++ )
						{
							if( playerNames[ i ].Equals( playerNames[ j ] ) )
							{
								Write( "That name already exists! Please choose a different name: ");
								nameCheck = true;
								playerNames[ i ] = ReadLine( ).Trim( ); 
								break;
							}
						}
					}
				}
				playerCoins[ i ] = 100;
			}
			WriteLine( );
		}
		
		public void ResetDeck( )
		{
			gameCards.FillDeck( );
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				players[ i ].ClearPlayer( );
				splits[ i ].ClearPlayer( );
			}
			dealer.ClearPlayer( );
			gameCardsUsed.cards.Clear( );
		}
		
		public void CollectBets( )
		{
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				if( playerCoins[ i ] > 0 ) 
				{
					if( players[ i ].IsHuman )
					{	
						while( true )
						{
							Write( $"{playerNames[ i ]}, you have {playerCoins[ i ]} coins. Please enter your bet: " );
							
							string response = ReadLine( );
							if( int.TryParse( response, out int val ) )
							{
								WriteLine( );
								currentBets[ i ] = val;
							
								if( currentBets[ i ] > 0 && currentBets[ i ] < playerCoins[ i ] ) break;
							}
							Write( "Please enter a positive integer less or equal to your total coins. " );
							
						}
					}
					else
					{
						currentBets[ i ] = (int) Math.Ceiling( playerCoins[ i ] / 5.0 );
						WriteLine( $"{playerNames[ i ]} has bet {currentBets[ i ]} coins!" );
					}
				}
			}  
		}
		
		public void DealRound( )
		{
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				if( currentBets[ i ] > 0 )
				{
					players[ i ].DealPlayerCard( ref gameCards, ref gameCardsUsed );
					
					WriteLine($"{playerNames[ i ]} has hand " );
					players[ i ].WritePlayerHand( );
					WriteLine( );
				}
			}
			
			dealer.DealPlayerCard( ref gameCards, ref gameCardsUsed );
			if( dealer.playerCards.Count > 1 ) gameCardsUsed.cards.RemoveAt( gameCardsUsed.cards.Count - 1 );
			
			WriteLine($"Dealer is showing " );
			dealer.WriteDealerHand( );
			WriteLine( );
			
		}
		
		public void MainRound( )
		{	
			//if( playerCoins[ index ] > 0 )
			//{
				if( dealer.HasInsurance( ) )
				{
					WriteLine( "Since Dealer is showing an Ace, insurance is offered. This side wage pays 2 to 1 if the dealer's hole card is any face card. " );
				
					for( int index = 0; index < NumberOfPlayers; index ++ )
					{
					
						if( players[ index ].IsHuman )
						{
							WriteLine( $"{playerNames[ index ]}, enter a bet if you would like insurance and anything else if you do not. " ); 
							string response = ReadLine( );
							
							int val;
							if( int.TryParse( response, out val ) )  
							{
								insuranceBets[ index ] = val;
							}
							
							else insuranceBets[ index ] = 0;
						}
						else WriteLine( $"{playerNames[ index ]} declines insurance." );
						
					}	
					WriteLine( );
				}
				WriteLine("Entering Main Round!");
				
				if( NumberOfPlayers - NumberOfAi - NumberOfSmartAi > 0 )
				{
					WriteLine("Surrendering is being offered. Recover half of your wager if you fold your hand. Enter 'yes' if you would like to.");
					for( int index = 0; index < NumberOfPlayers; index ++ )
					{
						if( players[ index ].IsHuman )
						{
							Write( $"{playerNames[ index ]}: ");
							string response = ReadLine( );
							if( response.Equals("yes") ) players[ index ].WantsToSurrender = true;
						}
					}
				}
				if( dealer.HasBlackJack( ) ) return;
				
				for( int index = 0; index < NumberOfPlayers; index ++ )
				{
					if( players[ index ].IsHuman && !players[ index ].WantsToSurrender )
					{
						if( players[ index ].HasBlackJack( ) )
						{
							WriteLine( $"{playerNames[ index ]}, you have BLACKJACK!" ); 
							WriteLine( );
						}
						
						while( ! ( players[ index ].HasBusted( ) || players[ index ].HasBlackJack( ) ) )
						{
							WriteLine( $"{playerNames[ index ]}, your hand has value { players[index].ValueOfHand }. Would you like to hit, double or pass?" );
							
							gameCardsVisible = gameCards;
							gameCardsVisible.cards.Add( dealer.playerCards[ 1 ] ) ;
							WriteLine( $"{playerNames[ index ]}, your hand has odds of busting: { players[ index ].OddsOfBust( gameCardsVisible, out double[ ] odds ):F5} " );
							
							string response = ReadLine( );
						
							if( response.Equals("hit") || response.Equals("double") )
							{
								players[ index ].DealPlayerCard( ref gameCards, ref gameCardsUsed );
								players[ index ].WriteLastCard( $"{playerNames[ index ]}" );
								
								WriteLine( $"{playerNames[ index ]}, your new hand has value { players[index].ValueOfHand }." );
								
								if( players[ index ].HasBusted( ) ) WriteLine( $"{playerNames[ index ]}, you have BUSTED!"); 
								
								if( response.Equals("double") ) 
								{
									currentBets[ index ] *= 2;
									if( currentBets[ index ] > playerCoins[ index ] )
									{
										WriteLine( $"{playerNames[ index ]}, since you only had {playerCoins[ index ]}, you now bet {playerCoins[ index ]}." );
										currentBets[ index ] = playerCoins[ index ];
									}
									else WriteLine( $"{playerNames[ index ]}, you now bet {currentBets[ index ]}." );
									WriteLine( );
									break;
								}
								
							}
							
							else if( response.Equals("pass") )
							{
								if( players[index].ValueOfHand >= 14 ) WriteLine( "Good Move!" );
								else WriteLine( "Hmmm, not sure you should've done that!" );
								WriteLine( );
								break;
							}
							
							else WriteLine( "Please enter either 'hit', 'double' or 'pass'." );
							WriteLine( );
						}
					}
					else if( !players[ index].IsHuman )
					{
						bool wantsToHit = false;
						bool wantsToDouble = false;
					
						int dealerUpCard = dealer.ValueOfDealerUpCard( );
						
						while( ! ( players[ index ].HasBusted( ) || players[ index ].HasBlackJack( ) ) )
						{
							WriteLine( $"{playerNames[ index ]} is thinking... " );
							
							if (playerNames[ index ].StartsWith("Ai") )
							{	
								int aiHandValue = players[ index ].ValueOfHand;
								
								if( usingAce )
								{
									if( dealerUpCard < 7 )
									{
										if( aiHandValue <= 15 ) wantsToHit = true;
										
										else if( aiHandValue <= 18 ) wantsToDouble = true;
										
									}
									else if( aiHandValue <= 18 ) wantsToHit = true; 
								}
								else
								{
									if( aiHandValue <= 8 ) wantsToHit = true;
									
									else if( aiHandValue == 9 ) 
									{
										if( dealerUpCard < 7 ) wantsToDouble = true;
										
										else wantsToHit = true; 
									}
									
									else if( aiHandValue <= 11 ) 
									{
										if( aiHandValue > dealerUpCard ) wantsToDouble = true;
										else wantsToHit = true;
									}
									
									else if( aiHandValue <= 16 && dealerUpCard >= 7) wantsToHit = true;
								}
							}
							
							else
							{
								gameCardsVisible = gameCards;
								gameCardsVisible.cards.Add( dealer.playerCards[ 1 ] ) ;
								WriteLine( $"{playerNames[ index ]}, your hand has odds of busting: { players[ index ].OddsOfBust( gameCardsVisible, out double[ ] odds ):F5} " );
								
								double[ ] possibleValues = dealer.DealerOutcomes( gameCardsUsed );
								wantsToHit = players[ index ].MoveDecider( odds, possibleValues, ref wantsToDouble );
							}
							
								
							if( wantsToHit && !wantsToDouble ) 
							{
								WriteLine( $"{playerNames[ index ]} has decided to hit!" ); 
								
								players[ index ].DealPlayerCard( ref gameCards, ref gameCardsUsed );
								players[ index ].WriteLastCard( $"{playerNames[ index ]}" );
								
								WriteLine( $"{playerNames[ index ]}, now has a hand of value { players[index].ValueOfHand }." );
								
								if( players[ index ].HasBusted( ) ) WriteLine( $"{playerNames[ index ]}, has BUSTED!"); 
							}
							
							else if( wantsToDouble )
							{
								WriteLine( $"{playerNames[ index ]} has decided to double!" ); 
								
								players[ index ].DealPlayerCard( ref gameCards, ref gameCardsUsed );
								players[ index ].WriteLastCard( $"{playerNames[ index ]}" );
								
								WriteLine( $"{playerNames[ index ]}, now has a hand of value { players[index].ValueOfHand }." );
								
								currentBets[ index ] *= 2;
								if( currentBets[ index ] > playerCoins[ index ] )
								{
									WriteLine( $"{playerNames[ index ]}, only had {playerCoins[ index ]}, so now bet {playerCoins[ index ]}." );
									currentBets[ index ] = playerCoins[ index ];
								}
								else WriteLine( $"{playerNames[ index ]}, now bet {currentBets[ index ]}." );
								WriteLine( );
								break;
							}			
							else // wantsToPass
							{
								WriteLine( $"{playerNames[ index ]} has decided to pass!" );
								WriteLine( );
								break;
							}
							WriteLine( );
						}
					}
				}
			//}
		}
			
		public void DealerRound( )
		{
			WriteLine( "Let's see what the dealer has! Remember, we are playing soft 17!" );
			dealer.WritePlayerHand( );
			
			WriteLine( );
			WriteLine( $"Dealer's hand has a value of {dealer.ValueOfHand}. " );
			
			while( dealer.ValueOfHand < 17 )
			{
				WriteLine( $"Dealer is drawing another card!" );
				dealer.DealPlayerCard( ref gameCards, ref gameCardsUsed );
				dealer.WriteLastCard( "Dealer" );
				WriteLine( $"Dealer's hand has a value of {dealer.ValueOfHand}. " );
				WriteLine( );
			}
			
			if( dealer.ValueOfHand > 21 ) WriteLine( "Dealer has busted!" );
			WriteLine( );
		}
			
 		public void CompareToDealer( )
 		{
			int dealerHand = dealer.ValueOfHand;
			for( int index = 0; index < NumberOfPlayers; index ++ )
			{
				if( currentBets[ index ] > 0 )
				{
					int handValue = players[ index ].ValueOfHand;
					if( !players[ index ].WantsToSurrender ) WriteLine($"{playerNames[ index ]} has a hand with a value of {handValue}.");
					
					if( players[ index ].WantsToSurrender )
					{
						WriteLine($"{playerNames[ index ]} loses half their wager due to surrending.");
						playerCoins[ index ] -= currentBets[ index ] / 2;
					}					
					else if( players[ index ].HasBlackJack( ) ) 
					{
						WriteLine( $"Since {playerNames[ index ]} has BlackJack, they earned triple their money!" ); 
						playerCoins[ index ] += currentBets[ index ] * 3 / 2; 
					}
					else if( handValue > 21 )
					{
						WriteLine( $"{playerNames[ index ]} has busted, losing their bet!" );
						playerCoins[ index ] -= currentBets[ index ];
					}
					else if( dealerHand > 21 )
					{
						WriteLine( $"Dealer has busted, so {playerNames[ index ]} doubles their money!" );
						playerCoins[ index ] += currentBets[ index ];
					}
					else if( handValue == dealerHand ) WriteLine( $"{playerNames[ index ]} and the Dealer both have hands with a value of {dealerHand}. Push! {playerNames[ index ]} gets their money back.");
					
					else if( handValue > dealerHand )
					{
						WriteLine( $"{playerNames[ index ]} doubles their money!" );
						playerCoins[ index ] += currentBets[ index ]; 
					}
					else 
					{
						WriteLine( $"The Dealer beat {playerNames[ index ]}. {playerNames[ index ]} loses their money!" );
						playerCoins[ index ] -= currentBets[ index ]; 
					}
					WriteLine( );
					
				}
			}
			
			if( dealer.HasInsurance( ) )
			{
				int makeMoney = -1;
				if( dealer.FulfillsInsurance( ) ) 
				{
					makeMoney = 2; 
					WriteLine( "Dealer's second card is a face card, so insurance pays off!" );
				}
				else WriteLine( "Dealer's second card is not a face card... Unlucky!" );
				
				for( int index = 0; index < NumberOfPlayers; index ++ )
				{
					if( insuranceBets[ index ] > 0 )
					{
						playerCoins[ index ] += makeMoney * insuranceBets[ index ]; 
						
						if( makeMoney == 2 ) WriteLine( $"{playerNames[ index ]} gains {makeMoney * insuranceBets[ index ]} from insurance." ); 
						else WriteLine( $"{playerNames[ index ]} loses {Math.Abs( makeMoney * insuranceBets[ index ] )} from insurance." ); 
					}
				}
			}
		}
		
		public void EndRound( )
		{
			WriteLine( "Here are the current totals for each player: " );
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				if( currentBets[ i ] > 0 )
				{
					WriteLine( $"{playerNames[ i ]}: {playerCoins[ i ]} coins." );
					if( playerCoins[ i ] == 0 )
					{
						WriteLine( );
						WriteLine( $"***{playerNames[ i ]} has been eliminated! They are out of money!***" );
						currentBets[ i ] = 0;
					}
					players[ i ].AceCount = 0;
					players[ i ].usingAce = false;
					players[ i ].playerCards.Clear( );
				}
			}
			WriteLine( );
			
		}
		
		public void EndGame( )
		{
			WriteLine( "Thank you for playing BlackJack! Here are the scores for the game:" ); 
			WriteLine( );
			
			int maxCoins = 0;
			int maxCoinsIndex = -1;
			bool isATie = false;
			
			if( CheckGame( ) == false ) 
			{
				WriteLine( "All players lost!" );
				return;
			}
			
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				WriteLine( $"{playerNames[ i ]}: {playerCoins[ i ]} coins." );
				if( maxCoins < playerCoins[ i ] )
				{
					maxCoins = playerCoins[ i ];
					maxCoinsIndex = i;
					isATie = false;
				}
				else if( maxCoins == playerCoins[ i ] ) isATie = true;
			}

			if( !isATie ) WriteLine( $"The Winner is: {playerNames[ maxCoinsIndex ]}" );  
			else 
			{
				Write( "There is a tie for first place!! Winners are:" );
				for( int i = maxCoinsIndex; i < NumberOfPlayers; i ++ )
				{
					if( playerCoins [ i ] == playerCoins[ maxCoinsIndex ] ) Write( $" {playerNames[ i ]}" );
				}
				Write( ".");
			}	
		}
		
		public bool CheckGame( )
		{
			bool playersRemaining = false;
			for( int i = 0; i < NumberOfPlayers; i ++ )
			{
				if( playerCoins[ i ] > 0 ) playersRemaining = true;
			}
			return playersRemaining;
		}
	}
}
