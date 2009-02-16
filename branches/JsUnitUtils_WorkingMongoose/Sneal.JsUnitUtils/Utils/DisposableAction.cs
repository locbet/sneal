﻿#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

namespace Sneal.JsUnitUtils.Utils
{
    /// <summary>
    /// Wraps a delegate all that is called on Dispose, used for wrapping
    /// an action from a method call.
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private Action action;

        public DisposableAction() {}

        public DisposableAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            if (action != null)
            {
                action();
            }
        }
    }

    public delegate void Action();
}