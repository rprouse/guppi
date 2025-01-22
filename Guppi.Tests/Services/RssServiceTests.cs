using System;
using System.Threading.Tasks;
using Guppi.Core.Services;
using NUnit.Framework;
using Shouldly;

namespace Guppi.Tests.Services;

public class RssServiceTests
{
    private RssService _rssService;
    [SetUp]
    public void Setup()
    {
        _rssService = new RssService();
    }

    [Test]
    public async Task ReadRssFeed_WhenCalledWithBbc_ParsesAndReturnsRssFeed()
    {
        var rssService = new RssService();
        var inputUri = "Rss/bbc_world_rss.xml";
        //inputUri = "https://feeds.bbci.co.uk/news/world/rss.xml";
        var feed = await rssService.ReadRssFeed(inputUri);

        feed.Title.ShouldBe("BBC News");
        feed.Description.ShouldBe("BBC News - World");
        feed.Link.ShouldBe("https://www.bbc.co.uk/news/world");
        feed.Items.Count.ShouldBe(28);

        var item = feed.Items[0];
        item.Id.ShouldBe("https://www.bbc.com/news/videos/cn4mwe3lk9wo#0");
        item.Title.ShouldBe("Bishop asks Trump to show mercy to LGBT people and migrants ");
        item.Description.ShouldBe("Right Rev. Mariann Edgar Budde made a plea to the president in a sermon that Trump later criticised.");
        item.Published.ShouldBe(new DateTimeOffset(2025, 1, 21, 19, 47, 14, TimeSpan.Zero));
        item.Content.ShouldBe("");
        item.Author.ShouldBe("");
        item.Link.ShouldBe("https://www.bbc.com/news/videos/cn4mwe3lk9wo");
    }

    [Test]
    public async Task ReadRssFeed_WhenCalledWithQC_ParsesAndReturnsRssFeed()
    {
        var rssService = new RssService();
        var inputUri = "Rss/QCRSS.xml";
        var feed = await rssService.ReadRssFeed(inputUri);

        feed.Title.ShouldBe("QC RSS");
        feed.Description.ShouldBe("The Official QC RSS Feed");
        feed.Link.ShouldBe("http://www.questionablecontent.net");
        feed.Items.Count.ShouldBe(166);

        var item = feed.Items[0];
        item.Id.ShouldBe("6D71FF55-6C29-4A46-A13D-AAC531C41AB1");
        item.Title.ShouldBe("So Much Luggage");
        item.Description.ShouldStartWith("<p><img src=\"http://www.questionablecontent.net/comics/5488.png\">");
        item.Published.ShouldBe(new DateTimeOffset(2025, 1, 20, 21, 52, 13, TimeSpan.FromHours(-4)));
        item.Content.ShouldBe("");
        item.Author.ShouldBe("");
        item.Link.ShouldBe("http://questionablecontent.net/view.php?comic=5488");
    }

    [Test]
    public async Task ReadRssFeed_WhenCalledWithLifehacker_ParsesAndReturnsCustomItems()
    {
        var rssService = new RssService();
        var inputUri = "Rss/lifehacker.rss";
        var feed = await rssService.ReadRssFeed(inputUri);

        feed.Title.ShouldBe("Lifehacker");
        feed.Description.ShouldBe("Do everything better.");
        feed.Link.ShouldBe("https://lifehacker.com/feed/rss");
        feed.Items.Count.ShouldBe(100);

        var item = feed.Items[0];
        item.Id.ShouldBe("01JJ53BPNDWH9249Z7B2N3R105");
        item.Title.ShouldBe("Instagram Reels Is Now Broadcasting Your Likes to Your Friends");
        item.Description.ShouldBe("And bad news if you want to turn it off.");
        item.Published.ShouldBe(new DateTimeOffset(2025, 1, 21, 22, 30, 00, TimeSpan.Zero));
        item.Content.ShouldBe("<p><a href=\"https://lifehacker.com/tech/current-status-of-the-tiktok-ban\" target=\"_blank\">TikTokers may have returned to their platform of choice</a>, but with the U.S. ban still on the books, they are facing an existential threat. Meanwhile, Instagram Reels users are encountering a problem of the platform's own making: Meta, in all its wisdom, is making it easier than ever for users to see who liked a particular reel. That means two things, of course: It's easier than ever to see which videos your friends have liked, but it's also easier than ever for your friends to see which videos <em>you </em>have liked. </p><p>Adam Mosseri, head of Instagram, <a href=\"https://www.instagram.com/reel/DE7eV_zxKbx/\" target=\"_blank\" title=\"open in a new window\" rel=\"noopener\">announced the changes in a post on Friday</a>. Here's what's new: When you launch Reels on Instagram, you may now see a bar appear in the top right corner of the screen where you'll find profile pictures of friends that have liked a particular reel. Tap this bar, and you'll be taken to another video feed, dubbed \"With friends.\" This feed exclusively features reels that your friends have liked. In my experience, most videos  in the feed have a like from exactly one friend, but now and again I'll see reels liked by multiple friends. </p><p>Whether the video has one like or three, you can tap on any user's profile picture to confirm, yes, they did like this reel, and to reply to them if you wish. There is also a bar on the bottom of the page that lets you reply to that user's like, or simply react with a laughing, crying, or \"in love\" emoji.</p><p>When you do reply to a like, it'll send your friend a DM. Here, they'll see a thumbnail of the video in question, with your message or emoji reaction below it. Above it, they'll see \"Replied to your like,\" so they'll know this was a direct reply to their like, rather than you sharing a video <em>you </em>liked with them.</p><h2 id=\"instagram-has-already-been-showing-likes-for-a-while\">Instagram has already been showing likes for a while</h2><p>Some of this experience isn't new. Anyone who has spent time scrolling on Reels knows that Instagram will frequently show you when one or more of your friends has liked a reel you're watching. However, this change puts those likes on display in a way they weren't before: Previously, running into friends' likes was a much more casual and passive process. Now, you can knowingly scroll through videos your friends liked, dramatically increasing the number of liked videos you'll encounter.</p><p>I suppose there's an argument that this change makes it easier to find content you might enjoy, since you presumably share some interests with your Instagram friends. However, I also another side of the argument&mdash;that making likes much more visible will encourage people to <em>stop</em> liking reels. Instagram Reels is a blackhole of content, and your fine-tuned algorithm probably shows you at least some videos <em>you </em>find interesting or funny, but you know other people won't. Or maybe you'd rather others not know you find them interesting or funny. There's a lot of <a href=\"https://lifehacker.com/entertainment/the-out-of-touch-adults-guide-to-kid-culture-brain-rot\" target=\"_blank\">brain rot humor</a> floating around out there (among other things), and I'd wager many wouldn't want to offer a glowing endorsement of each bizarre video that gets a reaction out of them.</p><h2 id=\"can-you-stop-friends-from-seeing-reels-you-liked-on-instagram\">Can you stop friends from seeing reels you liked on Instagram? </h2><p>If this change has you feeling anxious, bad news: This time, there doesn't appear to be a way to disable the feature. Instagram does let you hide likes on posts and reels from view, but the app doesn't offer an option to stop others from seeing which reels you've liked. For now, if you like a video, you should assume it's possible your friends will know about it at some point while scrolling through the app.</p>");
        item.Author.ShouldBe("Jake Peterson");
        item.Link.ShouldBe("https://lifehacker.com/tech/instagram-is-making-it-easier-to-see-which-reels-your-friends-liked?utm_medium=RSS");
    }
}
