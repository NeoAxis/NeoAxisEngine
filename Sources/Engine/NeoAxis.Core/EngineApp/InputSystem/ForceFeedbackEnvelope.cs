//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Engine
//{
//   public class ForceFeedbackEnvelope
//   {
//      ForceFeedbackEffect owner;

//      int attackLevel;
//      int attackTime;
//      int fadeLevel;
//      int fadeTime;
//      bool active;

//      //

//      internal ForceFeedbackEnvelope( ForceFeedbackEffect owner )
//      {
//         this.owner = owner;
//      }

//      /// <summary>
//      /// Amplitude for the start of the envelope, relative to the baseline, in the range from 0 through 1.
//      /// If the effect's type-specific data does not specify a baseline, the amplitude is relative to 0.
//      /// </summary>
//      public int AttackLevel
//      {
//         get { return attackLevel; }
//         set
//         {
//            if( attackLevel == value )
//               return;
//            attackLevel = value;

//            x;
//            //owner.SetNeedUpdateParameters();
//         }
//      }

//      /// <summary>
//      /// The time, in seconds, to reach the TermSustain level.
//      /// </summary>
//      public int AttackTime
//      {
//         get { return attackTime; }
//         set
//         {
//            if( attackTime == value )
//               return;
//            attackTime = value;

//            x;
//            //owner.SetNeedUpdateParameters();
//         }
//      }

//      /// <summary>
//      /// Amplitude for the end of the envelope, relative to the baseline, in the range from 0 through 1.
//      /// If the effect's type-specific data does not specify a baseline, the amplitude is relative to 0.
//      /// </summary>
//      public int FadeLevel
//      {
//         get { return fadeLevel; }
//         set
//         {
//            if( fadeLevel == value )
//               return;
//            fadeLevel = value;

//            x;
//            //owner.SetNeedUpdateParameters();
//         }
//      }

//      /// <summary>
//      /// The time, in seconds, to reach the fade level.
//      /// </summary>
//      public int FadeTime
//      {
//         get { return fadeTime; }
//         set
//         {
//            if( fadeTime == value )
//               return;
//            fadeTime = value;

//            x;
//            //owner.SetNeedUpdateParameters();
//         }
//      }

//      /// <summary>
//      /// Set true for enable envelope of the effect.
//      /// </summary>
//      public bool Active
//      {
//         get { return active; }
//         set
//         {
//            if( active == value )
//               return;
//            active = value;

//            x;
//            //owner.SetNeedUpdateParameters();
//         }
//      }
//   }
//}
