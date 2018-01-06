//
// Copyright (C) 2018 Authlete, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific
// language governing permissions and limitations under the
// License.
//


using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AuthorizationServer.Util
{
    /// <summary>
    /// Utility to render a view manually.
    /// </summary>
    public class Renderer
    {
        readonly Controller _controller;
        readonly IViewEngine _viewEngine;


        public Renderer(Controller controller, IViewEngine engine)
        {
            _controller = controller;
            _viewEngine = engine;
        }


        public async Task<string> Render(object model)
        {
            string viewName =
                _controller.ControllerContext.ActionDescriptor.ActionName;

            return await Render(viewName, model);
        }


        public async Task<string> Render(string viewName, object model)
        {
            _controller.ViewData.Model = model;

            var result = CreateViewEngineResult(viewName);

            using (var writer = new StringWriter())
            {
                // Prepare a context for rendering.
                var context = CreateViewContext(result, writer);

                // Render the view.
                await result.View.RenderAsync(context);

                // Convert the rendering result to a string.
                return writer.GetStringBuilder().ToString();
            }
        }


        ViewEngineResult CreateViewEngineResult(string viewName)
        {
            return _viewEngine.FindView(
                _controller.ControllerContext, viewName, false);
        }


        ViewContext CreateViewContext(
            ViewEngineResult result, TextWriter writer)
        {
            return new ViewContext(
                _controller.ControllerContext,
                result.View,
                _controller.ViewData,
                _controller.TempData,
                writer,
                new HtmlHelperOptions()
            );
        }
    }
}
