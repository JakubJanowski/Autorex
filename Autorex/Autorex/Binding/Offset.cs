﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autorex.Binding {
	public class Offset: Property {
		public ValueProperty X { get; } = new ValueProperty();
		public ValueProperty Y { get; } = new ValueProperty();
		public override string Visibility {
			set {
				X.Visibility = value;
				Y.Visibility = value;
			}
		}
	}
}