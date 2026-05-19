IEnumerable<Post> posts = (c.Posts as IEnumerable<Post>) ?? Enumerable.Empty<Post>();
// or
IEnumerable<Post> posts = ((IEnumerable<object>)c.SomeObject).Cast<Post>();