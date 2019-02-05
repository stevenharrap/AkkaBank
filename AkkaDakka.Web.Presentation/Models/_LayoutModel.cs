using System.Collections.Immutable;

namespace AkkaBank.Web.Presentation.Models
{
    public class Slides
    {
        private static readonly Slide[] _steps = {
            new Slide("Home", "~/Pages/index.cshtml"),
            new Slide("Actors", "~/Pages/actors.cshtml"),
            new Slide("Messages", "~/Pages/messages.cshtml"),
            new Slide("Example Actor", "~/Pages/exampleActor.cshtml"),
            new Slide("BasicBank - Awfully Simple", "~/Pages/ConsoleDirect.cshtml"),
            new Slide("BasicBank - ATM V1", "~/Pages/AtmV1.cshtml"),
            new Slide("Routers", "~/Pages/routers.cshtml"),
            new Slide("BasicBank - ATM V2", "~/Pages/AtmV2.cshtml"),
            new Slide("Clusters", "~/Pages/clusters.cshtml"),
            new Slide("Distributed Bank", "~/Pages/distributedBank.cshtml"),
            new Slide("Advertising With PubSub", "~/Pages/pubsub.cshtml"),
            new Slide("Account Fees With PubSub and Routing", "~/Pages/billing.cshtml"),
            new Slide("Finish", "~/Pages/finish.cshtml"),
        };

        public static Slide[] Get() => _steps;

        public static Slide Get(int number) => number >= 0 && number <= _steps.Length - 1
            ? _steps[number]
            : null;
    }

    public class _LayoutModel : _BlankModel
    {
        public ImmutableArray<Slide> Slides { get; }
        public int SlideNumber { get; }

        public int? PreviousSlide => SlideNumber - 1 >= 0
            ? SlideNumber - 1
            : (int?)null;
        public Slide CurrentSlide => Slides[SlideNumber];
        public int? NextSlide => SlideNumber + 1 < Slides.Length
            ? SlideNumber + 1
            : (int?)null;

        public _LayoutModel(Slide[] slides, int slideNumber) : base(Models.Slides.Get(slideNumber).Title)
        {
            Slides = slides.ToImmutableArray();
            SlideNumber = slideNumber;
        }
    }

    public class Slide
    {
        public string Title { get; }

        public string View { get; }

        public Slide(string title, string view)
        {
            Title = title;
            View = view;
        }
    }
}