using System;
using System.Collections;
using System.Threading.Tasks;
using Android.OS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Ouya.Console.Api;

namespace InAppPurchases
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        List<ButtonSprite> m_buttons = new List<ButtonSprite>();
        FocusManager m_focusManager = new FocusManager();
        private static string m_debugText = string.Empty;
        private ButtonSprite BtnGetProducts = null;
        private ButtonSprite BtnPurchase = null;
        private ButtonSprite BtnGetReceipts = null;
        private ButtonSprite BtnPause = null;
        private IOuyaResponseListener ListenerRequestProducts = null;
        private Task<IList<Product>> TaskRequestProducts = null;
        private Task<bool> TaskRequestPurchase = null;
        private Task<IList<Receipt>> TaskRequestReceipts = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            m_focusManager.OnClick += OnClick;

            base.Initialize();
        }

        public class RequestProductsListener : IOuyaResponseListener
        {
            public void Dispose()
            {
                
            }

            public IntPtr Handle
            {
                get { return Game1.Activity.Handle; }
            }

            public void OnCancel()
            {
                Game1.m_debugText = "OnCancel";
            }

            public void OnFailure(int errorCode, string errorMessage, Bundle optionalData)
            {
                Game1.m_debugText = "OnFailure";
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                Game1.m_debugText = "OnSuccess";
            }
        }

        private void OnClick(object sender, FocusManager.ClickEventArgs clickEventArgs)
        {
            if (null == clickEventArgs.Button)
            {
                return;
            }

            m_debugText = clickEventArgs.Button.Text;

            if (clickEventArgs.Button == BtnGetProducts)
            {
                m_debugText = "Fetching product list...";
                IList<Purchasable> purchasables = new List<Purchasable>()
                                                      {
                                                          new Purchasable("long_sword"),
                                                          new Purchasable("sharp_axe"),
                                                          new Purchasable("cool_level"),
                                                          new Purchasable("awesome_sauce"),
                                                          new Purchasable("__DECLINED__THIS_PURCHASE"),
                                                      };

                m_focusManager.SelectedProductIndex = 0;
                TaskRequestProducts = Activity1.PurchaseFacade.RequestProductList(purchasables);

                //ListenerRequestProducts = new RequestProductsListener();
                //Activity1.PurchaseFacade.RequestProductList(purchasables, ListenerRequestProducts);
            }

            else if (clickEventArgs.Button == BtnPurchase)
            {
                if (null != TaskRequestProducts &&
                    m_focusManager.SelectedProductIndex < TaskRequestProducts.Result.Count)
                {
                    Product product = TaskRequestProducts.Result[m_focusManager.SelectedProductIndex];
                    TaskRequestPurchase = Activity1.PurchaseFacade.RequestPurchase(product, product.Identifier);
                }
            }

            else if (clickEventArgs.Button == BtnGetReceipts)
            {
                m_focusManager.SelectedReceiptIndex = 0;
                TaskRequestReceipts = Activity1.PurchaseFacade.RequestReceipts();
            }
            else if (clickEventArgs.Button == BtnPause)
            {
                m_debugText = "Pause button pressed.";
                m_focusManager.SelectedButton = BtnPause;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("spriteFont1");

            BtnGetProducts = new ButtonSprite();
            BtnGetProducts.Initialize(font,
                Content.Load<Texture2D>("Graphics\\ButtonActive"),
                Content.Load<Texture2D>("Graphics\\ButtonInactive"));
            BtnGetProducts.Position = new Vector2(150, 200);
            BtnGetProducts.TextureScale = new Vector2(2f, 0.5f);
            BtnGetProducts.Text = "Get Product List";
            BtnGetProducts.TextOffset = new Vector2(40, 20);
            m_buttons.Add(BtnGetProducts);

            BtnPurchase = new ButtonSprite();
            BtnPurchase.Initialize(font,
                Content.Load<Texture2D>("Graphics\\ButtonActive"),
                Content.Load<Texture2D>("Graphics\\ButtonInactive"));
            BtnPurchase.Position = new Vector2(600, 200);
            BtnPurchase.TextureScale = new Vector2(2f, 0.5f);
            BtnPurchase.Text = "Request Purchase";
            BtnPurchase.TextOffset = new Vector2(40, 20);
            m_buttons.Add(BtnPurchase);

            BtnGetReceipts = new ButtonSprite();
            BtnGetReceipts.Initialize(font,
                Content.Load<Texture2D>("Graphics\\ButtonActive"),
                Content.Load<Texture2D>("Graphics\\ButtonInactive"));
            BtnGetReceipts.Position = new Vector2(1100, 200);
            BtnGetReceipts.TextureScale = new Vector2(1.5f, 0.5f);
            BtnGetReceipts.Text = "Get Receipts";
            BtnGetReceipts.TextOffset = new Vector2(30, 20);
            m_buttons.Add(BtnGetReceipts);

            BtnPause = new ButtonSprite();
            BtnPause.Initialize(font,
                Content.Load<Texture2D>("Graphics\\ButtonActive"),
                Content.Load<Texture2D>("Graphics\\ButtonInactive"));
            BtnPause.Position = new Vector2(1500, 200);
            BtnPause.TextureScale = new Vector2(1f, 0.5f);
            BtnPause.Text = "Pause";
            BtnPause.TextOffset = new Vector2(30, 20);
            m_buttons.Add(BtnPause);

            m_focusManager.SelectedButton = BtnGetProducts;
            m_focusManager.Mappings[BtnGetProducts] = new FocusManager.ButtonMapping()
                                                          {
                                                              Right = BtnPurchase
                                                          };
            m_focusManager.Mappings[BtnPurchase] = new FocusManager.ButtonMapping()
                                                       {
                                                           Left = BtnGetProducts,
                                                           Right = BtnGetReceipts
                                                       };
            m_focusManager.Mappings[BtnGetReceipts] = new FocusManager.ButtonMapping()
            {
                Left = BtnPurchase,
            };

            m_focusManager.Mappings[BtnPause] = new FocusManager.ButtonMapping()
            {
                Left = BtnGetReceipts,
            };
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            // touch exception property to avoid killing app
            if (null != TaskRequestPurchase)
            {
                AggregateException exception = TaskRequestProducts.Exception;
                if (null != exception)
                {
                    m_debugText = string.Format("Request Products has exception. {0}", exception);
                    TaskRequestProducts.Dispose();
                    TaskRequestProducts = null;
                }
            }

            // touch exception property to avoid killing app
            if (null != TaskRequestPurchase)
            {
                AggregateException exception = TaskRequestPurchase.Exception;
                if (null != exception)
                {
                    m_debugText = string.Format("Request Purchase has exception. {0}", exception);
                    TaskRequestPurchase.Dispose();
                    TaskRequestPurchase = null;
                }
            }

            // touch exception property to avoid killing app
            if (null != TaskRequestReceipts)
            {
                AggregateException exception = TaskRequestReceipts.Exception;
                if (null != exception)
                {
                    m_debugText = string.Format("Request Receipts has exception. {0}", exception);
                    TaskRequestReceipts.Dispose();
                    TaskRequestReceipts = null;
                }
            }

            // TODO: Add your update logic here
            m_focusManager.UpdateFocus();

            foreach (ButtonSprite button in m_buttons)
            {
                if (button == m_focusManager.SelectedButton)
                {
                    button.ButtonTexture = button.ButtonActive;
                }
                else
                {
                    button.ButtonTexture = button.ButtonInactive;
                }
            }

            if (m_focusManager.SelectedButton == BtnGetReceipts)
            {
                if (null != TaskRequestReceipts)
                {
                    if (TaskRequestReceipts.IsCanceled)
                    {
                        m_debugText = "Request Receipts has cancelled.";
                    }
                    else if (TaskRequestReceipts.IsCompleted)
                    {
                        m_debugText = "Request Receipts has completed.";
                    }
                }

                if (null != TaskRequestReceipts &&
                    !TaskRequestReceipts.IsCanceled &&
                    TaskRequestReceipts.IsCompleted)
                {
                    m_focusManager.UpdateReceiptFocus(TaskRequestReceipts.Result.Count);
                }
            }
            else
            {
                if (m_focusManager.SelectedButton == BtnPurchase)
                {
                    if (null != TaskRequestPurchase)
                    {
                        if (TaskRequestPurchase.IsCanceled)
                        {
                            m_debugText = "Request Purchase has cancelled.";
                        }
                        else if (TaskRequestPurchase.IsCompleted)
                        {
                            if (TaskRequestPurchase.Result)
                            {
                                m_debugText = "Request Purchase has completed succesfully.";
                            }
                            else
                            {
                                m_debugText = "Request Purchase has completed with failure.";
                            }
                        }
                    }
                }
                else
                {
                    if (null != TaskRequestProducts)
                    {
                        if (TaskRequestProducts.IsCanceled)
                        {
                            m_debugText = "Request Products has cancelled.";
                        }
                        else if (TaskRequestProducts.IsCompleted)
                        {
                            if (TaskRequestProducts.Result.Count > 0)
                            {
                                m_debugText = "Request Products has completed with results.";
                            }
                            else
                            {
                                m_debugText = "Request Products has completed without results.";
                            }
                        }
                    }
                }

                if (null != TaskRequestProducts &&
                    !TaskRequestProducts.IsCanceled &&
                    TaskRequestProducts.IsCompleted)
                {
                    m_focusManager.UpdateProductFocus(TaskRequestProducts.Result.Count);
                }
            }

            m_focusManager.UpdatePauseFocus(BtnPause);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, string.Format("Hello from MonoGame! {0}", m_debugText), new Vector2(100, 100), Color.White);
            spriteBatch.DrawString(font, "Use DPAD to switch between buttons | Press O to click the button", new Vector2(500, 170), Color.Orange);
            foreach (ButtonSprite button in m_buttons)
            {
                button.Draw(spriteBatch);
            }

            #region Products

            if (null != TaskRequestProducts &&
                !TaskRequestProducts.IsCanceled &&
                TaskRequestProducts.IsCompleted)
            {
                Vector2 position = new Vector2(140, 300);
                for (int index = 0; index < TaskRequestProducts.Result.Count; ++index)
                {
                    Product product = TaskRequestProducts.Result[index];
                    spriteBatch.DrawString(font, string.Format("Product: {0}", product.Identifier), position, index == m_focusManager.SelectedProductIndex ? Color.Orange : Color.White);
                    position += new Vector2(0, 20);
                }
            }

            #endregion

            #region Receipts

            if (null != TaskRequestReceipts &&
                !TaskRequestReceipts.IsCanceled &&
                TaskRequestReceipts.IsCompleted)
            {
                Vector2 position = new Vector2(1120, 300);
                for (int index = 0; index < TaskRequestReceipts.Result.Count; ++index)
                {
                    Receipt receipt = TaskRequestReceipts.Result[index];
                    spriteBatch.DrawString(font, string.Format("Receipt: {0}", receipt.Identifier), position, index == m_focusManager.SelectedReceiptIndex ? Color.Orange : Color.White);
                    position += new Vector2(0, 20);
                }
            }

            #endregion

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}