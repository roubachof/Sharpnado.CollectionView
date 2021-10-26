using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.CollectionView.RenderedViews
{
    public class SizedDataTemplate : DataTemplate
    {
        public SizedDataTemplate(DataTemplate dataTemplate, double size)
        {
            DataTemplate = dataTemplate;
            Size = size;
        }

        public DataTemplate DataTemplate { get; set; }

        public double Size { get; set; }
    }

    public class SizedDataTemplateExtension : IMarkupExtension<SizedDataTemplate>
    {
        public double Size { get; set; }

        public DataTemplate Template { get; set; }

        public SizedDataTemplate ProvideValue(IServiceProvider serviceProvider)
        {
            return new SizedDataTemplate(Template, Size);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}