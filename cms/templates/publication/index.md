---
title: "{{ title }}"
subtitle: "{{ type_for_venue }}"
{% if authors %}authors: [ {% for author in authors %}"{{ author }}"{% if not loop.last %}, {% endif %}{% endfor %} ]{% endif %}
{% if authors %}tags: [ {% for tag in tags %}"{{ tag }}"{% if not loop.last %}, {% endif %}{% endfor %} ]{% endif %}
category: Publications
---

{{ abstract }}

Links:

- [Paper (local copy)]({attach}paper.pdf)
{% if doi %}- [Paper (doi)](https://doi.org/{{ doi }}){% endif %}
