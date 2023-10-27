﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Interfaces
{
    public interface IDamageable
    {
        public float Health { get; set; }
        public void OnHit(float damage, Vector2 knockback);
        public void OnHit(float damage);
        public bool Targetable { get; set; }
        public bool Invincible { get; set; }
        public void OnObjectDestroyed();
        public float GetHealth();
        public int GetExp();

    }
}
