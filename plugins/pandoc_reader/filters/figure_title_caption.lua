function Figure(fig)
    local image = fig.content and fig.content[1] and fig.content[1].content and fig.content[1].content[1]
    if image.t == "Image" and image.title then
        fig.caption.short = pandoc.Str(image.title)
        fig.caption.long = pandoc.Blocks(pandoc.Plain(pandoc.Str(image.title)))
    end
    return fig
end
