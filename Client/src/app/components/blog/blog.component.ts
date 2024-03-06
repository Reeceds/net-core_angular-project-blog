import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post.service';

@Component({
  selector: 'app-blog',
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css'],
})
export class BlogComponent implements OnInit {
  posts: Post[] = [];

  constructor(private _postService: PostService) {}

  ngOnInit(): void {
    this.getPosts();
  }

  getPosts() {
    return this._postService.GetPosts().subscribe({
      next: (response) => {
        this.posts = response;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
}
