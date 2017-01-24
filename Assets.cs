using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LeyStoneEngine;
using LeyStoneEngine.Utility;
using LeyStoneEngine.Interface;
using LeyStoneEngine.Graphics;

using Microsoft.Xna.Framework.Content;

namespace LeyStoneGame
{
    public class Assets : BaseAssets
    {
        public override void Load(GraphicsDevice device, ContentManager manager)
        {
            LoadWhitePixel(device, manager);

            base.Load(device, manager);

            textures.Add("porto", manager.Load<Texture2D>("Textures/Porto"));
            textures.Add("test", manager.Load<Texture2D>("Textures/test"));
            textures.Add("glowMask", manager.Load<Texture2D>("Textures/glowMask"));

            textures.Add("bolt", manager.Load<Texture2D>("Textures/bolt"));

            EffectContainer sharpen = new EffectContainer(manager.Load<Effect>("Effects/sharpen"), EffectUse.Pre);
            effectContainers.Add("sharpen", sharpen);

            EffectContainer chromaticAbberation = new EffectContainer(manager.Load<Effect>("Effects/chromaticAbberation"), EffectUse.Pre);
            effectContainers.Add("chromaticAbberation", chromaticAbberation);
        }
    }
}
