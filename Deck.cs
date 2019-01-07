using System;
using static System.Console;

using System.Collections.Generic;
using BlackJackCardSuit;
using BlackJackCards;

namespace BlackJackDeck
{	
	class Deck
	{
		public List< Tuple< Cards, CardSuit > > cards;
		private Random rGen; 
		
		public Deck( )
		{
			cards = new List< Tuple< Cards, CardSuit > >( );
		}
		
		public void FillDeck( )
		{
			for( int i = 0; i < 52; i ++ )
			{
				if( i < 13 )
				{
					cards.Add( Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), i + 1 ) ), Enum.Parse<CardSuit>( "Hearts" ) ) );
				}
				else if( i < 26 ) 
				{
					cards.Add( Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), i % 13 + 1 ) ), Enum.Parse<CardSuit>( "Diamonds" ) ) );
				}
				else if( i < 39 )
				{
					cards.Add( Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), i % 13 + 1 ) ), Enum.Parse<CardSuit>( "Clubs" ) ) );
				}
				
				else
				{
					cards.Add( Tuple.Create( Enum.Parse<Cards>( Enum.GetName( typeof(Cards), i % 13 + 1 ) ), Enum.Parse<CardSuit>( "Spades" ) ) );
				}
			}
		
		}
		
		public Tuple< Cards, CardSuit > RandomCard( )
		{
			rGen = new Random( );
			int indexToRemove = rGen.Next( cards.Count );
			
			Tuple< Cards, CardSuit > returnedCard = cards[ indexToRemove ];
			cards.RemoveAt( indexToRemove );
			
			return returnedCard;
		}
	
		public override string ToString( )
		{
			string result = "";
			foreach( Tuple< Cards, CardSuit > cardTuple in cards )
			{
				result += Environment.NewLine + cardTuple;
			}
			return result;
		}
	}
}
