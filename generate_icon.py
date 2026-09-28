from PIL import Image, ImageDraw, ImageFilter

def create_app_icon():
    # Supersampled resolution (1024x1024)
    canvas_size = 1024
    img = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))

    # Outer soft ambient shadow
    margin = 88
    radius = 192
    shadow = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))
    s_draw = ImageDraw.Draw(shadow)
    s_draw.rounded_rectangle(
        [margin, margin + 28, canvas_size - margin, canvas_size - margin + 28],
        radius=radius,
        fill=(0, 0, 0, 110)
    )
    shadow = shadow.filter(ImageFilter.GaussianBlur(32))
    img.paste(shadow, (0, 0), shadow)

    # Base Squircle Mask
    mask = Image.new("L", (canvas_size, canvas_size), 0)
    mask_draw = ImageDraw.Draw(mask)
    mask_draw.rounded_rectangle(
        [margin, margin, canvas_size - margin, canvas_size - margin],
        radius=radius,
        fill=255
    )

    # Vertical Fluent Gradient (Vibrant Azure -> Rich Royal Blue)
    grad = Image.new("RGBA", (canvas_size, canvas_size))
    for y in range(canvas_size):
        ratio = (y - margin) / float(canvas_size - 2 * margin)
        ratio = max(0.0, min(1.0, ratio))
        # Top: #388bfd (56, 139, 253) -> Mid: #1f6feb (31, 111, 235) -> Bottom: #094eb8 (9, 78, 184)
        r = int(56 * (1 - ratio) + 9 * ratio)
        g = int(145 * (1 - ratio) + 80 * ratio)
        b = int(255 * (1 - ratio) + 200 * ratio)
        for x in range(canvas_size):
            grad.putpixel((x, y), (r, g, b, 255))

    img.paste(grad, (0, 0), mask)

    # Inner Glass Highlight Border
    border = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))
    b_draw = ImageDraw.Draw(border)
    b_draw.rounded_rectangle(
        [margin, margin, canvas_size - margin, canvas_size - margin],
        radius=radius,
        outline=(255, 255, 255, 75),
        width=5
    )
    img.paste(border, (0, 0), border)

    # Markdown Symbol Layer
    symbol = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))
    sym_draw = ImageDraw.Draw(symbol)

    # Geometry for clean solid Markdown M and Arrow
    # M: polygon
    # Left bar, center V, right bar
    # Base coordinates
    m_left = 230
    m_right = 530
    m_top = 340
    m_bottom = 680
    w = 64  # bar thickness

    # Draw M with polygons for crisp geometric perfection
    # Left vertical bar
    sym_draw.rounded_rectangle([m_left, m_top, m_left + w, m_bottom], radius=16, fill=(255, 255, 255, 255))
    # Right vertical bar
    sym_draw.rounded_rectangle([m_right - w, m_top, m_right, m_bottom], radius=16, fill=(255, 255, 255, 255))
    # Diagonal left: (m_left, m_top) -> (m_left+w, m_top) -> (center, center_y+w) -> (center, center_y)
    center_x = (m_left + m_right) // 2
    center_y = 520
    diag_pts = [
        (m_left + w - 4, m_top + 16),
        (center_x, center_y),
        (m_right - w + 4, m_top + 16),
        (m_right - w + 4, m_top + 80),
        (center_x, center_y + 64),
        (m_left + w - 4, m_top + 80),
    ]
    sym_draw.polygon(diag_pts, fill=(255, 255, 255, 255))

    # Arrow Down
    arr_x = 710
    arr_w = 64
    arr_top = 340
    arr_bottom = 570
    # Stem
    sym_draw.rounded_rectangle([arr_x - arr_w // 2, arr_top, arr_x + arr_w // 2, arr_bottom], radius=14, fill=(255, 255, 255, 255))
    # Arrow head (triangle)
    head_pts = [
        (arr_x - 110, 550),
        (arr_x + 110, 550),
        (arr_x, 680)
    ]
    sym_draw.polygon(head_pts, fill=(255, 255, 255, 255))

    # Symbol soft drop shadow
    sym_shadow = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))
    ss_draw = ImageDraw.Draw(sym_shadow)
    ss_draw.rounded_rectangle([m_left, m_top + 12, m_left + w, m_bottom + 12], radius=16, fill=(0, 0, 0, 80))
    ss_draw.rounded_rectangle([m_right - w, m_top + 12, m_right, m_bottom + 12], radius=16, fill=(0, 0, 0, 80))
    ss_draw.polygon([(p[0], p[1] + 12) for p in diag_pts], fill=(0, 0, 0, 80))
    ss_draw.rounded_rectangle([arr_x - arr_w // 2, arr_top + 12, arr_x + arr_w // 2, arr_bottom + 12], radius=14, fill=(0, 0, 0, 80))
    ss_draw.polygon([(p[0], p[1] + 12) for p in head_pts], fill=(0, 0, 0, 80))
    sym_shadow = sym_shadow.filter(ImageFilter.GaussianBlur(12))

    img.paste(sym_shadow, (0, 0), sym_shadow)
    img.paste(symbol, (0, 0), symbol)

    # Downsample using Lanczos for flawless anti-aliasing
    final_img = img.resize((512, 512), Image.Resampling.LANCZOS)
    final_img.save("src/Resources/app.png", format="PNG")

    # Multi-resolution ICO
    icon_sizes = [(256, 256), (128, 128), (64, 64), (48, 48), (32, 32), (16, 16)]
    final_img.save("src/Resources/app.ico", format="ICO", sizes=icon_sizes)
    print(f"Generated flawless app.ico with sizes: {icon_sizes}")

if __name__ == "__main__":
    create_app_icon()
