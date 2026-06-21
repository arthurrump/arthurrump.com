<!-- .slide: data-background="linear-gradient(to bottom right, #000, rgb(1, 79, 101))" -->

# Requirements for an<br/>Automated Assessment Tool for<br/>Learning Programming by Doing

<!-- .element: style="font-size: 1.3em;" -->

<small>

Paper for ICST2025\
Presented at the FMT Colloquium on 2025-03-20

**Arthur Rump**, Vadim Zaytsev, Angelika Mader\
University of Twente

</small>

Notes:
Title lie

Colloquium:
- Extended edition
- Feel free to interrupt

***

<!-- .slide: data-background="rgb(19, 64, 21)" -->

## context

why and where do we want automated assessment?

Notes:
And what even is assessment?

---

<!-- .slide: data-background="rgb(19, 64, 21)" -->

### assessment

<div class="columns">
<div class="fragment" data-fragment-index="1">

#### formative

"feedback"

<!-- .element: class="fragment" data-fragment-index="4" -->

actually useful

<!-- .element: class="fragment" data-fragment-index="6" -->

</div>
<div class="fragment" data-fragment-index="2">

#### summative

"grades"

<!-- .element: class="fragment" data-fragment-index="3" -->

required

<!-- .element: class="fragment" data-fragment-index="5" -->

</div>
</div>

Notes:
oversimplified summaries

spend more time in formative, then we look at the same work multiple times and students would like feedback quickly

---

<!-- .slide: data-background="rgb(19, 64, 21)" -->


### how about testing?

Notes:
works well for small, well-defined exercises in programming intro courses

we want students to learn about program design

---

<!-- .slide: data-background="rgb(19, 64, 21)" -->


### open-ended assignments

<div class="columns" style="font-size: .8em">
<div class="fragment">

#### TCS M2<br/>Programming Project

build a board game

client/server architecture

from scratch

</div>
<div class="fragment">

#### CreaTe M4<br/>Algorithms

"build something"

interactive

use the building blocks

</div>
</div>

Notes:
- TCS:

  there is some networking code they can use, but they design their architecture

  there is a protocol, but they design the user interface

- CreaTe:

  zero functional requirements

  no given interfaces at all

why? to learn about design. assessed on object-oriented design: structure, encapsulation etc.

---

<!-- .slide: data-background="rgb(19, 64, 21)" -->


### so<br/><span class="fragment">how about testing?</span>

Notes:
there are some things we can do regarding code style (linters)

but open-endedness is not something these tools can typically deal with

what should a tool look like?

***

<!-- .slide: data-background="rgb(57, 45, 98)" -->

## interviews

<div class="fragment">
with teachers, TAs, students, exam board, educational support

<small>$n = 12$</small>
</div>

Notes:
edu support &rarr; CELT

limitation: all UT, strong project-based learning culture, involved with the courses mentioned

what did we learn?

---

<!-- .slide: data-background="rgb(57, 45, 98)" -->

### use cases

<table class="noborder">
    <tr class="fragment"><td rowspan="3" style="vertical-align: top">giving feedback</td><td class="fragment" style="padding-bottom: 0">to individuals</td></tr>
    <tr class="fragment"><td style="padding-top: 0; padding-bottom: 0;">to the cohort</td></tr>
    <tr class="fragment"><td style="padding-top: 0;">to subgroups</td></tr>
    <tr class="fragment"><td colspan="2" style="text-align: center">grading</td></tr>
    <tr class="fragment"><td colspan="2" style="text-align: center">evaluation</td></tr>
</table>

Notes:
so, with these use cases, how did our participants think the tool should work

---

<!-- .slide: data-background="rgb(98, 45, 82)" -->

### don't build an autograder

use automation to support assessment

<!-- .element: class="fragment" -->

Notes:
half of our participants mentioned they don't actually want automated assessment

summative: responsibility

formative: curation, turning feedback into actionable feedback

---

<!-- .slide: data-background="rgb(57, 45, 98)" -->

### requirements <!-- .element: style="font-size: 1em;" -->

<div class="columns">
<div class="fragment">

#### supporting <!-- .element: style="font-size: 1.3em;" -->

during assessment

<!-- .element: class="fragment" -->

during configuration

<!-- .element: class="fragment" -->

</div>
<div class="fragment">

#### flexible <!-- .element: style="font-size: 1.3em;" -->

extensible

<!-- .element: class="fragment" -->

configurable

<!-- .element: class="fragment" -->

</div>
</div>

Notes:
- supporting
  - during assessment: don't replace
  - during configuration: help with defining assessment, from ILOs onward
- flexible
  - extensible: different languages, LMSs
  - configurable: all criteria

***

<!-- .slide: data-background="rgb(1, 79, 101)" -->

## what's next?

---

<!-- .slide: data-background="rgb(1, 79, 101)" -->

### gradual automation?

can we use manual assessment to configure automation?

Notes:
to give support during configuration

---

<!-- .slide: data-background="rgb(1, 79, 101)" -->

### combining manual and automated assessment?

*automated &rarr; manual* is easy

<!-- .element: class="fragment" -->

what about *manual &rarr; automated*?

<!-- .element: class="fragment" -->

Notes:
to create flexibility

automated &rarr; manual beyond just doing whatever and handing over, but showing the relevant information for manual assessment

just to calculate the grade, but also cases where something is easy to manually identify, then automatically analyse

---

<!-- .slide: data-background="rgb(1, 79, 101)" -->

### two perspectives on assessment <!-- .element: style="font-size: 1em;" -->

<div class="columns">
<div class="fragment">

#### answering questions <!-- .element: style="font-size: 1.3em;" -->

</div>
<div class="fragment">

#### gathering evidence <!-- .element: style="font-size: 1.3em;" -->

</div>
</div>

Notes:
answering questions may provide some structure to build a combination of manual and automated asessment

gathering evidence may gather the data necessary to do gradual automation

***

<!-- .slide: class="full-height" data-background="linear-gradient(to top left, #000, rgb(1, 79, 101))" -->

not an autograder

<!-- .element: class="fragment" -->

a tool should be **supporting** and **flexible**

<!-- .element: class="fragment" -->

through gradual automation?

<!-- .element: class="fragment" -->

through a combination of manual and automated assessment?

<!-- .element: class="fragment" -->

<div class="fragment" style="position: absolute; right: 24px; bottom: 24px; text-align: right; --r-link-color: var(--r-main-color); font-size: .6em;">

Arthur Rump\
arthur.rump@utwente.nl

<!-- .element: style="margin: 0" -->

</div>

***

<!-- .slide: data-visibility="uncounted" -->

bonus slides:\
selected criteria from rubrics

---

<!-- .slide: data-visibility="uncounted" -->

from the TCS M2 Programming Project

**Encapsulation**

<!-- .element: style="text-align: left;" -->

<dl style="margin-left: 0">
    <dt>sufficient</dt>
    <dd>All fields except constants are private.</dd>
    <dt>good<span style="font-weight: normal">, additionally:</span></dt>
    <dd>Methods are only public when intended to be used from other classes.</dd>
</dl>

Notes:
the first is easy to automate

the second requires judgement of intention, but an overview of public methods and usages would help

---

<!-- .slide: data-visibility="uncounted" -->

from the TCS M2 Programming Project\
(older version)

<div style="text-align: left;">

**Structure**

Interfaces are applied where appropriate to ensure low coupling between components.

</div>

Notes:
components may not be clearly defined, so that could be hard to automate

determining coupling is much easier to automate, once components are identified

---

<!-- .slide: data-visibility="uncounted" -->

from the TCS M2 Programming Project

**Tests**

<!-- .element: style="text-align: left;" -->

<dl style="font-size: .9em; margin-left: 0;">
  <dt>subpar</dt>
  <dd>There are non-trivial game logic tests that check common execution flows and gameover conditions.</dd>
  <dt>sufficient</dt>
  <dd>
    Tests cover common execution flows and edge cases of game logic, with at least 75% line coverage of methods related to moves and gameover conditions.<br/>
    Tests have at least 90% line coverage of game logic code.
  </dd>
</dl>

***

<!-- .slide: data-visibility="uncounted" -->

## how about LLMs?

Notes:
from what I've seen, the results are quite mixed

---

<!-- .slide: data-visibility="uncounted" -->

### requirements for assessment <!-- .element: style="font-size: 1em" -->

<div style="display: flex; justify-content: space-around; font-size: 1.3em;">

**reliability**

**validity**

**transparency**

</div>

Notes:
how does a stochastic parrot fare?

validity is fine

reliability could be a risk, depending on your definition

transparency: it's a black box, we don't want *computer says no*

---

<!-- .slide: data-visibility="uncounted" -->

### is it all bad?

no, but use it within a system

Notes:
a system that provides guide rails regarding reliability and transparency
