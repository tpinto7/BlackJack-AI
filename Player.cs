using System;
using static System.Console;

using System.Collections.Generic;
using BlackJackDeck;
using BlackJackCardSuit;
using BlackJackCards;

namespace BlackJackPlayer
{
	class Player : Deck
	{
		public List< Tuple< Cards, CardSuit > > playerCards;
		
		public bool IsHuman{ get; set; }
		public int AceCount{ get; set; }
		public bool usingAce = false;
		public int valueOfHand;
		public bool WantsToSurrender{ get; set; }
		
		public Player( ) 
		{
			playerCards = new List< Tuple< Cards, CardSuit > >( );
		}
		
		public void ClearPlayer( ) 
		{
			playerCards.Clear( );
			AceCount = 0;
			usingAce = false;
			valueOfHand = 0;
		}
		
		public void DealPlayerCard( ref Deck cards, ref Deck gameCardsUsed) 
		{
			Tuple< Cards, CardSuit > dealedCard = cards.RandomCard( );
			playerCards.Add( dealedCard );
			gameCardsUsed.cards.Add( dealedCard );
		}
		
		public void WritePlayerHand( )
		{
			foreach( Tuple< Cards, CardSuit > cardTuple in playerCards )
			{
				WriteLine( cardTuple.Item1 + " Of " + cardTuple.Item2);
			}
		}
		
		public void WriteLastCard( string player )
		{
			Tuple< Cards, CardSuit > lastCard = playerCards[ playerCards.Count - 1 ];
			WriteLine( $"{player} has drawn {lastCard.Item1} Of {lastCard.Item2}. " );
		}
		
		public void WriteDealerHand( )
		{
			WriteLine( playerCards[ 0 ].Item1 + " Of " + playerCards[ 0 ].Item2 );
			if( this.HasBlackJack( ) ) 
			{
				WriteLine( playerCards[ 1 ].Item1 + " Of " + playerCards[ 1 ].Item2 );
				WriteLine( "The Dealer has BlackJack!" );
			}
		}
		
		public int ValueOfDealerUpCard( )
		{
			int cardValue = (int) playerCards[ 0 ].Item1;
				
			if( cardValue > 10 ) cardValue = 10;
			else if( cardValue == 1 ) cardValue = 11;
			
			return cardValue;
		}
		
		public int ValueOfHand
		{
			get
			{
				valueOfHand = 0;
				usingAce = false;
				foreach( Tuple< Cards, CardSuit > cardTuple in this.playerCards ) 
				{
					int cardValue = (int) cardTuple.Item1;
					
					if( cardValue >= 10 ) valueOfHand += 10;
					else if( cardValue == 1 ) 
					{
						valueOfHand++;
						AceCount++;
					}
					else valueOfHand += cardValue;
				}
				if( AceCount > 0 && valueOfHand <= 11 ) 
				{
					valueOfHand += 10;
					usingAce = true;
				}
				return valueOfHand;
			}
			set
			{
				valueOfHand = value;
			}
		}
		
		public bool HasBusted( )
		{
			return this.ValueOfHand > 21;
		}
		
		public bool HasBlackJack( )
		{
			return ( this.playerCards.Count == 2 && this.ValueOfHand == 21 );
		}
		
		public bool IsSplit( ) 
		{
			int card1 = ( int ) this.playerCards[ 0 ].Item1;
			int card2 = ( int ) this.playerCards[ 1 ].Item1;
			return ( card1 == card2 || ( card1 >= 10 && card2 >= 10 ) );
		} 
		
		public Tuple< Cards, CardSuit > MakeSplit( ) 
		{
			Tuple< Cards, CardSuit > newHand = this.playerCards[ 0 ];
			this.playerCards.RemoveAt( 0 );
			return newHand;
		}
		
		public bool HasInsurance( )
		{
			int ace = ( int ) this.playerCards[ 0 ].Item1;
			return ( ace == 1 );
		}
		
		public bool FulfillsInsurance( )
		{
			int card = ( int ) this.playerCards[ 1 ].Item1; 
			return ( card > 10 );
		}
		
		public double OddsOfBust( Deck gameCards, out double[ ] outcomes ) // Bust is automatic loss.
		{
			double cardsRemaining = gameCards.cards.Count;
			double bustCount = 0;
			outcomes = new double[ 7 ];
			
			foreach( Tuple< Cards, CardSuit > cardTuple in gameCards.cards )
			{
				Player tempPlayer = new Player( );
				
				foreach( Tuple< Cards, CardSuit > playerCard in this.playerCards )
				{
					tempPlayer.playerCards.Add( playerCard );
				}
				tempPlayer.playerCards.Add( cardTuple );
				if( tempPlayer.HasBusted( ) ) bustCount++;
				
				else 
				{
					switch( tempPlayer.ValueOfHand )
					{
						case 17: 
							outcomes[ 1 ]++;
							break;
						case 18: 
							outcomes[ 2 ]++;
							break;
						case 19:
							outcomes[ 3 ]++;
							break;
						case 20: 
							outcomes[ 4 ]++;
							break;
						case 21: 
							outcomes[ 5 ]++;
							break;
						default:
							outcomes[ 0 ]++;
							break;
					}
				}
				
			}
			outcomes[ 6 ] = bustCount;
			
			for( int i = 0; i < outcomes.Length; i ++ )
			{
				outcomes[ i ] /= cardsRemaining;
			}
			return outcomes[ 6 ];
		}
		
		public double[ ] DealerOutcomes( Deck gameCardsUsed )
		{
			double[ ] possibleValues = new double[ 7 ];
			
			double[ ] quantityOfDeck = { 4, 4, 4, 4, 4, 4, 4, 4, 4, 16 };
			double[ ] valueOfDeck =    { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			int cardsRemaining = 52 - gameCardsUsed.cards.Count;
			
			foreach( Tuple< Cards, CardSuit > cardPlayed in gameCardsUsed.cards )
			{
				int cardValue = ( int ) cardPlayed.Item1;
				if( cardValue > 10 ) cardValue = 10;
				quantityOfDeck[ cardValue - 1 ]--;
			}
			
			for( int i = 0; i < quantityOfDeck.Length && quantityOfDeck[ i ] > 0 ; i ++ )
			{
				Player tempPlayer = new Player( );
				tempPlayer.playerCards.Add( this.playerCards[ 0 ] );
				
				Tuple< Cards, CardSuit > drawnCard = Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), i + 1 ) ), Enum.Parse<CardSuit>( "Hearts" ) );
				tempPlayer.playerCards.Add( drawnCard );
				cardsRemaining--;
				if( tempPlayer.ValueOfHand >= 17 )
				{
						ImplementSwitch( ref possibleValues, tempPlayer.ValueOfHand, quantityOfDeck[ i ], 5, cardsRemaining );
				}
				
				else
				{
					cardsRemaining--;
					quantityOfDeck[ i ] --;
					for( int j = 0; j < quantityOfDeck.Length && quantityOfDeck[ j ] > 0; j ++ )
					{
						Tuple< Cards, CardSuit > secondDrawnCard = Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), j + 1 ) ), Enum.Parse<CardSuit>( "Hearts" ) );
						tempPlayer.playerCards.Add( secondDrawnCard );
										
						if( tempPlayer.ValueOfHand >= 17 )
						{	
							ImplementSwitch( ref possibleValues, tempPlayer.ValueOfHand, quantityOfDeck[ j ], 4, cardsRemaining );
						}
						else
						{
							quantityOfDeck[ j ] --;
							cardsRemaining--;
							for( int k = 0; k < quantityOfDeck.Length && quantityOfDeck[ k ] > 0; k ++ )
							{
								Tuple< Cards, CardSuit > thirdDrawnCard = Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), k + 1 ) ), Enum.Parse<CardSuit>( "Hearts" ) );
								tempPlayer.playerCards.Add( thirdDrawnCard );
												
								if( tempPlayer.ValueOfHand >= 17 )
								{	
									ImplementSwitch( ref possibleValues, tempPlayer.ValueOfHand, quantityOfDeck[ k ], 3, cardsRemaining );
								}
							
								else
								{
									quantityOfDeck[ k ] --;
									cardsRemaining--;
									for( int l = 0; l < quantityOfDeck.Length && quantityOfDeck[ l ] > 0; l ++ )
									{
										Tuple< Cards, CardSuit > fourthDrawnCard = Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), l + 1 ) ), Enum.Parse<CardSuit>( "Hearts" ) );
										tempPlayer.playerCards.Add( fourthDrawnCard );
														
										if( tempPlayer.ValueOfHand >= 17 )
										{	
											ImplementSwitch( ref possibleValues, tempPlayer.ValueOfHand, quantityOfDeck[ l ], 2, cardsRemaining );
										}
									
										else
										{
											quantityOfDeck[ l ] --;
											cardsRemaining--;
											for( int m = 0; m < quantityOfDeck.Length && quantityOfDeck[ m ] > 0; m ++ )
											{
												Tuple< Cards, CardSuit > fifthDrawnCard = Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), m + 1 ) ), Enum.Parse<CardSuit>( "Hearts" ) );
												tempPlayer.playerCards.Add( fifthDrawnCard );
													
												if( tempPlayer.ValueOfHand >= 17 )
												{	
													ImplementSwitch( ref possibleValues, tempPlayer.ValueOfHand, quantityOfDeck[ m ], 1, cardsRemaining );
												}
											
												else
												{
													possibleValues[ 6 ] += quantityOfDeck[ l ];
												}
												tempPlayer.playerCards.RemoveAt( 5 );
											}
											cardsRemaining++;
											quantityOfDeck[ l ] ++;
										}
										tempPlayer.playerCards.RemoveAt( 4 );
									}
									cardsRemaining++;
									quantityOfDeck[ k ]++;
								}
								tempPlayer.playerCards.RemoveAt( 3 );
							}
							cardsRemaining++;
							quantityOfDeck[ j ]++;
						}
						tempPlayer.playerCards.RemoveAt( 2 );
					}
					cardsRemaining++;
					quantityOfDeck[ i ]++;
				}
				tempPlayer.ClearPlayer( );
			}
			
			double sum = 0;
			for( int i = 0; i < possibleValues.Length; i ++ )
			{
				sum += possibleValues[ i ];
			}
			for( int i = 0; i < possibleValues.Length; i ++ )
			{
				possibleValues[ i ] /= sum;
				// WriteLine( possibleValues[ i ] );
			}
			
			return possibleValues;
		}
		// Make method that can do this... Modifications are just to switch statement( Math.Pow to input weighting... )
		
		public void ImplementSwitch( ref double[ ] possibleValues, int handValue, double quantity, int power, int cards )
		{
			switch( handValue )
			{
				case 17:
					possibleValues[ 0 ]+= quantity * Math.Pow( 10 , power );
					break;
				case 18:
					possibleValues[ 1 ]+= quantity * Math.Pow( 10 , power );
					break;
				case 19:
					possibleValues[ 2 ]+= quantity * Math.Pow( 10 , power );
					break;
				case 20:
					possibleValues[ 3 ]+= quantity * Math.Pow( 10 , power );
					break;
				case 21: 
					possibleValues[ 4 ]+= quantity * Math.Pow( 10 , power );
					break;
				default:
					possibleValues[ 5 ]+= quantity * Math.Pow( 10 , power );
					break;
			}
		}
		
		public bool MoveDecider( double[ ] playerOutcomes, double[ ] dealerOutcomes, ref bool hitDouble)
		{
			hitDouble = false;
			
			double oddsWin = 0, oddsLose = 0, oddsTie = 0; 
			double oddsWinHit = 0, oddsLoseHit = 0, oddsTieHit = 0;
			
			oddsWin += dealerOutcomes[ 5 ];
			int handValue = this.ValueOfHand;
			
			for( int i = 0; i < 5; i ++ )
			{
				if( handValue == i + 17 ) oddsTie += dealerOutcomes[ i ];
				else if( handValue < i + 17  ) oddsLose += dealerOutcomes[ i ];
				else if( handValue > i + 17 ) oddsWin += dealerOutcomes[ i ];
				
			}
			WriteLine( );
			WriteLine( $"Odds win: {oddsWin} ");
			WriteLine( $"Odds lose: {oddsLose} ");
			WriteLine( $"Odds tie: {oddsTie} ");
			WriteLine( $"Ratio: { oddsWin / oddsLose } " );
			WriteLine( );
			
			oddsLoseHit += playerOutcomes[ 6 ];
			
			for( int i = 0; i < 6; i ++ )
			{
				for( int j = 0; j < 6; j ++ )
				{
					if( j + 1 < 6 && j + 1== i )
					{
						oddsTieHit += playerOutcomes[ i ] * dealerOutcomes[ j ];
					}
					
					else if( j < 5 && j >= i )
					{
						oddsLoseHit += playerOutcomes[ i ] * dealerOutcomes[ j ];
					}
					else
					{
						oddsWinHit += playerOutcomes[ i ] * dealerOutcomes[ j ];
					}
				}
			}
			WriteLine( $"Odds win if hit: {oddsWinHit} ");
			WriteLine( $"Odds lose if hit: {oddsLoseHit} ");
			WriteLine( $"Odds tie if hit: {oddsTieHit} ");
			WriteLine( $"Ratio if hit: { oddsWinHit / oddsLoseHit } " );
			WriteLine( );
			
			double ratioWin = oddsWin / oddsLose;
			double ratioWinHit = oddsWinHit / oddsLoseHit;
			
			bool shouldHit = ratioWinHit >= ratioWin ? true : false ;
			if( shouldHit && ratioWinHit > 1 && handValue >= 9 ) hitDouble = true;
			
			if( this.playerCards.Count == 2) this.WantsToSurrender =  Math.Max( ratioWinHit, ratioWin ) < 0.25 ? true : false;
			if( this.WantsToSurrender ) WriteLine( $"Smart Ai has decided to surrender" );
			
			return shouldHit;
		}
	}
}
