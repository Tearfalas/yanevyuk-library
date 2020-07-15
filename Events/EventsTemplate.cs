using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Category Template for Events //

/// <summary>
/// Changing the class name will break the functionality of the OnEvent attribute.
/// The class and its variables should NOT be public. You do not have to instantiate this class,
/// it is automatic.
/// </summary>

namespace YanevyukLibrary.Events{
		public class Events
		{
				public static Events Instance;

				public static Event example1;
				public static Event<int> example2;


		}
}

