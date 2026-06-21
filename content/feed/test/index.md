---
slug: test
date: 2026-06-21
title: "Syntax highlighting test"
tags: [ ".NET", "F#", "FAKE" ]
categories: [Posts]
---

## Python

```python
def fibonacci(n: int) -> list[int]:
    """Generate the first n Fibonacci numbers."""
    if n <= 0:
        return []
    if n == 1:
        return [0]
    fib = [0, 1]
    for i in range(2, n):
        fib.append(fib[i - 1] + fib[i - 2])
    return fib

@dataclass
class Point:
    x: float
    y: float

    def distance_to(self, other: "Point") -> float:
        return ((self.x - other.x) ** 2 + (self.y - other.y) ** 2) ** 0.5
```

## TypeScript

```typescript {linenos=inline}
interface Repository<T extends { id: string }> {
  get(id: string): Promise<T | undefined>;
  save(entity: T): Promise<void>;
}

class InMemoryRepository<T extends { id: string }> implements Repository<T> {
  private items = new Map<string, T>();

  async get(id: string): Promise<T | undefined> {
    return this.items.get(id);
  }

  async save(entity: T): Promise<void> {
    this.items.set(entity.id, entity);
  }
}
```

## F#

```fsharp
type Tree<'a> =
    | Leaf of 'a
    | Node of Tree<'a> * Tree<'a>

let rec depth tree =
    match tree with
    | Leaf _ -> 1
    | Node (left, right) -> 1 + max (depth left) (depth right)

let rec map f tree =
    match tree with
    | Leaf x -> Leaf (f x)
    | Node (left, right) -> Node (map f left, map f right)
```

## C\#

```csharp
public record Student(Guid Id, string Name, int Age);

public class Classroom
{
    private readonly Dictionary<Guid, Student> _students = new();

    public void Enroll(Student student)
    {
        if (student.Age < 18)
            throw new InvalidOperationException("Student must be 18 or older.");
        _students[student.Id] = student;
    }

    public IEnumerable<Student> SortedByName() =>
        _students.Values.OrderBy(s => s.Name).ToList();
}
```

## Clojure

```clojure
(defn fib [n]
  (loop [a 0 b 1 i 0]
    (if (= i n)
      a
      (recur b (+ a b) (inc i)))))

(defmacro unless [pred & body]
  `(if (not ~pred) (do ~@body)))

(->> (range 10)
     (map (fn [x] (* x x)))
     (filter even?)
     (reduce +))
```

## Prolog

```prolog
parent(tom, bob).
parent(bob, ann).
parent(bob, pat).

ancestor(X, Y) :- parent(X, Y).
ancestor(X, Z) :- parent(X, Y), ancestor(Y, Z).

sibling(X, Y) :-
    parent(Z, X),
    parent(Z, Y),
    X \= Y.
```

## YAML

```yaml
name: arthurrump.com
languages:
  - python
  - typescript
  - fsharp
  - csharp
  - clojure
  - prolog
build:
  command: hugo --minify
  output: public/
deploy:
  provider: cloudflare-pages
  project: arthurrump-com
```
