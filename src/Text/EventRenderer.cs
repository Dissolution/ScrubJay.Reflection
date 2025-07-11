namespace ScrubJay.Reflection.Text;

[PublicAPI]
public sealed class EventRenderer : Renderer<EventInfo>
{
    public override void RenderTo(EventInfo? @event, TextBuilder builder)
    {
        if (@event is not null)
        {
            var (prefix, postfix) = @event.NullabilityInfo().GetPrefixPostfix();

            builder
                .IfNotNull(prefix, static (tb, pf) => tb.Append(pf).Append(' '))
                .IfAppend(@event.IsStatic(), "static ")
                .Render(@event.EventHandlerType)
                .Append(postfix)
                .Append(' ')
                .Render(@event.OwnerType())
                .Append('.')
                .Append(@event.Name);
        }
    }
}