using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VanillaCookingExpanded
{
    public static class NumberUtility
    {
        private const float _tolerance = 0.0001f;

        public static bool EqualsTo(this float source, float target)
        {
            return Mathf.Abs(source - target) < _tolerance;
        }
    }
}
