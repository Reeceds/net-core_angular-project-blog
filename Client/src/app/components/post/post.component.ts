import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post.service';

import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css'],
})
export class PostComponent implements OnInit {
  clickedPost: Post | undefined;
  faCehvronLeft = faChevronLeft;

  constructor(
    private _postService: PostService,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.getSelectedPost();
  }

  getSelectedPost(): void {
    // Number converts the id captured from URL into a number
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this._postService.getPost(id).subscribe({
      next: (res) => {
        this.clickedPost = res;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  goBack(): void {
    this.location.back();
  }
}
